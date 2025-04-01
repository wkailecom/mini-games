# -*- coding: utf-8 -*-
import ctypes
import sys

STD_INPUT_HANDLE = -10
STD_OUTPUT_HANDLE= -11
STD_ERROR_HANDLE = -12

FOREGROUND_BLACK = 0
FOREGROUND_BLUE = 1
FOREGROUND_GREEN = 2
FOREGROUND_RED = 4
FOREGROUND_INTENSITY = 8

BACKGROUND_BLUE = 16
BACKGROUND_GREEN = 32
BACKGROUND_RED = 64
BACKGROUND_INTENSITY = 128

class ColoredPrint:
    # r"""' See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winprog/winprog/windows_api_reference.asp\n    for information on Windows APIs.'"""

    def set_cmd_color(self, color):
        # """(color) -> bit
        # Example: set_cmd_color(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE | FOREGROUND_INTENSITY)
        # """
        if sys.platform == "darwin":
            # print("这是macOS 操作系统")
            handle = None
        else:
            handle = ctypes.windll.kernel32.GetStdHandle(STD_OUTPUT_HANDLE)
            return  ctypes.windll.kernel32.SetConsoleTextAttribute(handle, color)
    
    def reset_color(self):
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE)

    def print_red_text(self, print_text):
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_INTENSITY)
        print(print_text)
        self.reset_color()

    def print_green_text(self, print_text):
        self.set_cmd_color(FOREGROUND_GREEN | FOREGROUND_INTENSITY)
        print(print_text)
        self.reset_color()

    def print_blue_text(self, print_text):
        self.set_cmd_color(FOREGROUND_BLUE | FOREGROUND_INTENSITY)
        print(print_text)
        self.reset_color()

    def print_red_text_with_blue_bg(self, print_text):
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_INTENSITY | BACKGROUND_BLUE | BACKGROUND_INTENSITY)
        print(print_text)
        self.reset_color()

