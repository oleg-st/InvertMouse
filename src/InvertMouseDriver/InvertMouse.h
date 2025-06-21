#pragma once

#include <ntddk.h>
#include <wdf.h>
#include <kbdmou.h>

#if DBG
#define DebugPrint(_x_) DbgPrint _x_
#else
#define DebugPrint(_x_)
#endif

#define NTDEVICE_NAME         L"\\Device\\invertmouse"
#define SYMBOLIC_NAME_STRING  L"\\DosDevices\\invertmouse"

// IOCTL codes for getting and setting settings
#define IOCTL_INVERTMOUSE_GET_SETTINGS CTL_CODE(FILE_DEVICE_MOUSE, 0x800, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_INVERTMOUSE_SET_SETTINGS CTL_CODE(FILE_DEVICE_MOUSE, 0x801, METHOD_BUFFERED, FILE_ANY_ACCESS)

typedef struct _INVERTMOUSE_SETTINGS {
    BOOLEAN enable;
    double multiplier_x;
    double multiplier_y;
} INVERTMOUSE_SETTINGS, * PINVERTMOUSE_SETTINGS;

// Device extension holds our per-device state and upper driver callback
typedef struct _DEVICE_EXTENSION {
    CONNECT_DATA UpperConnectData;
} DEVICE_EXTENSION, * PDEVICE_EXTENSION;

WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(DEVICE_EXTENSION, FilterGetData)

EXTERN_C_START

DRIVER_INITIALIZE DriverEntry;

EVT_WDF_DRIVER_DEVICE_ADD EvtDeviceAdd;
EVT_WDF_IO_QUEUE_IO_INTERNAL_DEVICE_CONTROL EvtIoInternalDeviceControl;
EVT_WDF_IO_QUEUE_IO_DEVICE_CONTROL InvertMouseControl;
VOID InvertMouseInit(WDFDRIVER);
NTSTATUS CreateControlDevice(WDFDRIVER);

VOID InvertMouseCallback(
    IN PDEVICE_OBJECT DeviceObject,
    IN PMOUSE_INPUT_DATA InputDataStart,
    IN PMOUSE_INPUT_DATA InputDataEnd,
    IN OUT PULONG InputDataConsumed
);

VOID WriteDelay(VOID);

VOID DispatchPassThrough(
    _In_ WDFREQUEST Request,
    _In_ WDFIOTARGET Target
);

EXTERN_C_END
