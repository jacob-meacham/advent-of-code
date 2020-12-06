cmake_minimum_required(VERSION 3.13)
set(CMAKE_C_STANDARD 11)
project(day_2 C)

enable_language(C ASM)

set (SOURCES
        main.asm)

add_executable(day_2 ${SOURCES})