#include "InvertMouse.h"

#ifdef ALLOC_PRAGMA
#pragma alloc_text (INIT, DriverEntry)
#pragma alloc_text (INIT, InvertMouseInit)
#pragma alloc_text (INIT, CreateControlDevice)
#pragma alloc_text (PAGE, EvtDeviceAdd)
#pragma alloc_text (PAGE, EvtIoInternalDeviceControl)
#pragma alloc_text (PAGE, InvertMouseControl)
#endif

struct {
    bool initialized;
    INVERTMOUSE_SETTINGS settings;
} global = {};

extern "C" PULONG InitSafeBootMode;

// Main callback where mouse packets are filtered
VOID
InvertMouseCallback(
    IN PDEVICE_OBJECT DeviceObject,
    IN PMOUSE_INPUT_DATA InputDataStart,
    IN PMOUSE_INPUT_DATA InputDataEnd,
    IN OUT PULONG InputDataConsumed
)
{
    WDFDEVICE hDevice = WdfWdmDeviceGetWdfDeviceHandle(DeviceObject);
    PDEVICE_EXTENSION devExt = FilterGetData(hDevice);

    auto num_packets = InputDataEnd - InputDataStart;

    // Only process if there is at least one packet, all are relative, and filter is enabled
    if (num_packets > 0 &&
        !(InputDataStart->Flags & MOUSE_MOVE_ABSOLUTE) &&
        global.settings.enable)
    {
        for (auto it = InputDataStart; it != InputDataEnd; ++it) {
            // Only modify packets with actual movement
            if (it->LastX != 0)
                it->LastX = static_cast<LONG>(it->LastX * global.settings.multiplier_x);
            if (it->LastY != 0)
                it->LastY = static_cast<LONG>(it->LastY * global.settings.multiplier_y);
        }
    }

    // Always call the upper driver's service callback with (potentially modified) packets
    (*(PSERVICE_CALLBACK_ROUTINE)devExt->UpperConnectData.ClassService)(
        devExt->UpperConnectData.ClassDeviceObject,
        InputDataStart,
        InputDataEnd,
        InputDataConsumed
        );;
}

// Applies a 1-second delay (for anti-cheat protection when multipliers change)
VOID
WriteDelay()
{
    LARGE_INTEGER interval;
    interval.QuadPart = -10000 * 1000; // 1 second in 100-ns intervals (negative for relative time)
    KeDelayExecutionThread(KernelMode, FALSE, &interval);
}

// Handles IOCTLs for getting/setting filter settings
_Use_decl_annotations_
VOID
InvertMouseControl(
    WDFQUEUE         Queue,
    WDFREQUEST       Request,
    size_t           OutputBufferLength,
    size_t           InputBufferLength,
    ULONG            IoControlCode
)
{
    UNREFERENCED_PARAMETER(Queue);
    UNREFERENCED_PARAMETER(InputBufferLength);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    PAGED_CODE();

    DebugPrint(("InvertMouse: Ioctl received into filter control object.\n"));

    if (!global.initialized) {
        WdfRequestCompleteWithInformation(Request, STATUS_CANCELLED, 0);
        return;
    }

    NTSTATUS status = STATUS_SUCCESS;
    size_t bytes_out = 0;

    switch (IoControlCode) {
    case IOCTL_INVERTMOUSE_GET_SETTINGS: {
        PINVERTMOUSE_SETTINGS out;
        status = WdfRequestRetrieveOutputBuffer(Request, sizeof(INVERTMOUSE_SETTINGS), (PVOID*)&out, NULL);
        if (NT_SUCCESS(status)) {
            *out = global.settings;
            bytes_out = sizeof(INVERTMOUSE_SETTINGS);
        }
        break;
    }
    case IOCTL_INVERTMOUSE_SET_SETTINGS: {
        PINVERTMOUSE_SETTINGS in;
        status = WdfRequestRetrieveInputBuffer(Request, sizeof(INVERTMOUSE_SETTINGS), (PVOID*)&in, NULL);
        if (NT_SUCCESS(status)) {
            // Apply delay only when multipliers change
            if (in->multiplier_x != global.settings.multiplier_x ||
                in->multiplier_y != global.settings.multiplier_y)
            {
                WriteDelay();
            }
            global.settings = *in;
        }
        break;
    }
    default:
        status = STATUS_INVALID_DEVICE_REQUEST;
        break;
    }

    WdfRequestCompleteWithInformation(Request, status, bytes_out);
}

// Creates a named control device for user-mode communication
NTSTATUS
CreateControlDevice(WDFDRIVER Driver)
{
    PWDFDEVICE_INIT             pInit = NULL;
    WDFDEVICE                   controlDevice = NULL;
    WDF_IO_QUEUE_CONFIG         ioQueueConfig;
    NTSTATUS                    status;
    WDFQUEUE                    queue;
    DECLARE_CONST_UNICODE_STRING(ntDeviceName, NTDEVICE_NAME);
    DECLARE_CONST_UNICODE_STRING(symbolicLinkName, SYMBOLIC_NAME_STRING);

    DebugPrint(("InvertMouse: Creating Control Device\n"));

    //
    //
    // In order to create a control device, we first need to allocate a
    // WDFDEVICE_INIT structure and set all properties.
    //
    pInit = WdfControlDeviceInitAllocate(
        Driver,
        &SDDL_DEVOBJ_SYS_ALL_ADM_RWX_WORLD_RW_RES_R
    );

    if (pInit == NULL) {
        status = STATUS_INSUFFICIENT_RESOURCES;
        goto Error;
    }

    //
    // Set exclusive to false so that more than one app can talk to the
    // control device simultaneously.
    //
    WdfDeviceInitSetExclusive(pInit, FALSE);

    status = WdfDeviceInitAssignName(pInit, &ntDeviceName);

    if (!NT_SUCCESS(status)) {
        goto Error;
    }

    status = WdfDeviceCreate(&pInit,
        WDF_NO_OBJECT_ATTRIBUTES,
        &controlDevice);

    if (!NT_SUCCESS(status)) {
        goto Error;
    }

    //
    // Create a symbolic link for the control object so that usermode can open
    // the device.
    //

    status = WdfDeviceCreateSymbolicLink(controlDevice, &symbolicLinkName);

    if (!NT_SUCCESS(status)) {
        goto Error;
    }

    //
    // Configure the default queue associated with the control device object
    // to be Serial so that request passed to RawaccelControl are serialized.
    //

    WDF_IO_QUEUE_CONFIG_INIT_DEFAULT_QUEUE(&ioQueueConfig,
        WdfIoQueueDispatchSequential);

    ioQueueConfig.EvtIoDeviceControl = InvertMouseControl;

    //
    // Framework by default creates non-power managed queues for
    // filter drivers.
    //
    status = WdfIoQueueCreate(controlDevice,
        &ioQueueConfig,
        WDF_NO_OBJECT_ATTRIBUTES,
        &queue // pointer to default queue
    );
    if (!NT_SUCCESS(status)) {
        goto Error;
    }

    //
    // Control devices must notify WDF when they are done initializing.   I/O is
    // rejected until this call is made.
    //
    WdfControlFinishInitializing(controlDevice);

    return STATUS_SUCCESS;

Error:

    if (pInit != NULL) WdfDeviceInitFree(pInit);

    if (controlDevice != NULL) {
        //
        // Release the reference on the newly created object, since
        // we couldn't initialize it.
        //
        WdfObjectDelete(controlDevice);
    }

    DebugPrint(("InvertMouse: CreateControlDevice failed with status code 0x%x\n", status));

    return status;
}

// Main driver entry point (DriverEntry)
NTSTATUS
DriverEntry(
    IN  PDRIVER_OBJECT  DriverObject,
    IN  PUNICODE_STRING RegistryPath
)
{
    WDF_DRIVER_CONFIG config;
    NTSTATUS status;
    WDFDRIVER driver;

    DebugPrint(("InvertMouse: DriverEntry.\n"));
    DebugPrint(("InvertMouse: Built %s %s\n", __DATE__, __TIME__));

    WDF_DRIVER_CONFIG_INIT(
        &config,
        EvtDeviceAdd
    );

    status = WdfDriverCreate(DriverObject,
        RegistryPath,
        WDF_NO_OBJECT_ATTRIBUTES,
        &config,
        &driver);

    if (NT_SUCCESS(status)) {
        if (*InitSafeBootMode == 0) {
            InvertMouseInit(driver);
        }
    }
    else {
        DebugPrint(("InvertMouse: WdfDriverCreate failed with status 0x%x\n", status));
    }

    return status;
}

VOID
InvertMouseInit(WDFDRIVER driver)
{
    NTSTATUS status;

    status = CreateControlDevice(driver);

    if (!NT_SUCCESS(status)) {
        DebugPrint(("InvertMouse: CreateControlDevice failed with status 0x%x\n", status));
        return;
    }

    global.settings.enable = false;
    global.settings.multiplier_x = 1.0;
    global.settings.multiplier_y = 1.0;
    global.initialized = true;
}

// Handles creation of filter device and initializes context
NTSTATUS
EvtDeviceAdd(
    IN WDFDRIVER        Driver,
    IN PWDFDEVICE_INIT  DeviceInit
)
{
    WDF_OBJECT_ATTRIBUTES deviceAttributes;
    NTSTATUS status;
    WDFDEVICE hDevice;
    WDF_IO_QUEUE_CONFIG ioQueueConfig;

    UNREFERENCED_PARAMETER(Driver);

    PAGED_CODE();

    DebugPrint(("InvertMouse: Enter EvtDeviceAdd\n"));

    if (!global.initialized) {
        DebugPrint(("InvertMouse: Not initialized\n"));
        return STATUS_SUCCESS;
    }

    // Mark device as a filter
    WdfFdoInitSetFilter(DeviceInit);

    WdfDeviceInitSetDeviceType(DeviceInit, FILE_DEVICE_MOUSE);

    WDF_OBJECT_ATTRIBUTES_INIT_CONTEXT_TYPE(&deviceAttributes,
        DEVICE_EXTENSION);

    status = WdfDeviceCreate(&DeviceInit, &deviceAttributes, &hDevice);
    if (!NT_SUCCESS(status)) {
        DebugPrint(("InvertMouse: WdfDeviceCreate failed with status code 0x%x\n", status));
        return status;
    }

    WDF_IO_QUEUE_CONFIG_INIT_DEFAULT_QUEUE(&ioQueueConfig, WdfIoQueueDispatchParallel);
    ioQueueConfig.EvtIoInternalDeviceControl = EvtIoInternalDeviceControl;

    status = WdfIoQueueCreate(hDevice,
        &ioQueueConfig,
        WDF_NO_OBJECT_ATTRIBUTES,
        WDF_NO_HANDLE);
    if (!NT_SUCCESS(status)) {
        DebugPrint(("InvertMouse: WdfIoQueueCreate failed 0x%x\n", status));
        return status;
    }

    DebugPrint(("InvertMouse: EvtDeviceAdd finished\n"));
    return status;
}

// Handles internal device control requests (IRP_MJ_INTERNAL_DEVICE_CONTROL)
VOID
EvtIoInternalDeviceControl(
    IN WDFQUEUE      Queue,
    IN WDFREQUEST    Request,
    IN size_t        OutputBufferLength,
    IN size_t        InputBufferLength,
    IN ULONG         IoControlCode
)
{
    PDEVICE_EXTENSION devExt;
    PCONNECT_DATA connectData;
    NTSTATUS status = STATUS_SUCCESS;
    WDFDEVICE hDevice;
    size_t length;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    PAGED_CODE();

    DebugPrint(("InvertMouse: Enter EvtIoInternalDeviceControl\n"));

    if (!global.initialized) {
        DebugPrint(("InvertMouse: Not initialized\n"));
        WdfRequestCompleteWithInformation(Request, STATUS_CANCELLED, 0);
        return;
    }

    hDevice = WdfIoQueueGetDevice(Queue);
    devExt = FilterGetData(hDevice);

    switch (IoControlCode) {
    case IOCTL_INTERNAL_MOUSE_CONNECT: {
        DebugPrint(("InvertMouse: CONNECT\n"));

        // Allow only one connection
        if (devExt->UpperConnectData.ClassService != NULL) {
            DebugPrint(("InvertMouse: SHARING\n"));
            status = STATUS_SHARING_VIOLATION;
            break;
        }

        status = WdfRequestRetrieveInputBuffer(Request, sizeof(CONNECT_DATA),
            reinterpret_cast<PVOID*>(&connectData), &length);
        if (!NT_SUCCESS(status)) {
            DebugPrint(("InvertMouse: WdfRequestRetrieveInputBuffer failed %x\n", status));
            break;
        }

        // Save the upper callback and substitute our own
        devExt->UpperConnectData = *connectData;

        connectData->ClassDeviceObject = WdfDeviceWdmGetDeviceObject(hDevice);
        connectData->ClassService = InvertMouseCallback;

        DebugPrint(("InvertMouse: CONNECT finished\n"));

        break;
    }
    case IOCTL_INTERNAL_MOUSE_DISCONNECT:
        // Not implemented: optional in most filter drivers
        status = STATUS_NOT_IMPLEMENTED;
        break;
    default:
        break;
    }

    if (!NT_SUCCESS(status)) {
        WdfRequestComplete(Request, status);
        return;
    }

    DispatchPassThrough(Request, WdfDeviceGetIoTarget(hDevice));
}

// Passes the request down to the next driver in the stack
VOID
DispatchPassThrough(
    _In_ WDFREQUEST Request,
    _In_ WDFIOTARGET Target
)
{
    //
    // Pass the IRP to the target
    //

    WDF_REQUEST_SEND_OPTIONS options;
    BOOLEAN ret;
    NTSTATUS status = STATUS_SUCCESS;

    //
    // We are not interested in post processing the IRP so 
    // fire and forget.
    //
    WDF_REQUEST_SEND_OPTIONS_INIT(&options,
        WDF_REQUEST_SEND_OPTION_SEND_AND_FORGET);

    ret = WdfRequestSend(Request, Target, &options);

    if (ret == FALSE) {
        status = WdfRequestGetStatus(Request);
        DebugPrint(("InvertMouse: WdfRequestSend failed: 0x%x\n", status));
        WdfRequestComplete(Request, status);
    }

    return;
}
