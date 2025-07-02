#pragma once

#define VERSION_MAJOR    1
#define VERSION_MINOR    0
#define VERSION_BUILD    0

#define VERSION         VERSION_MAJOR,VERSION_MINOR,VERSION_BUILD

#define STR_HELPER(x)   #x
#define STR(x)          STR_HELPER(x)

#define VERSION_STR     STR(VERSION_MAJOR) "." STR(VERSION_MINOR) "." STR(VERSION_BUILD)
