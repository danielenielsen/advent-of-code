const std = @import("std");
const fs = std.fs;

pub fn main() !void {
    const file_name = "../input.txt";
    const buffer = try readFile(file_name, std.heap.page_allocator);

    var sum: u64 = 0;
    var first_digit: ?u8 = null;
    var second_digit: ?u8 = null;

    for (buffer) |char| {
        if (char == '\n') {
            const first_digit_val = asciiDigitToDigit(first_digit.?);
            if (second_digit != null) {
                const second_digit_val = asciiDigitToDigit(second_digit.?);
                sum += first_digit_val * 10 + second_digit_val;
            } else {
                sum += first_digit_val * 10 + first_digit_val;
            }

            first_digit = null;
            second_digit = null;
        }

        if (isDigit(char)) {
            if (first_digit == null) {
                first_digit = char;
            } else {
                second_digit = char;
            }
        }
    }

    std.debug.print("Sum: {d}\n", .{sum});
}

fn isDigit(char: u8) bool {
    if (char >= 48 and char <= 57) {
        return true;
    }

    return false;
}

fn asciiDigitToDigit(char: u8) u64 {
    return char - 48;
}

fn readFile(file_name: []const u8, allocator: std.mem.Allocator) ![]u8 {
    const file = try fs.cwd().openFile(file_name, .{});
    defer file.close();

    const file_stats = try file.stat();
    const file_size = file_stats.size;
    const buffer = try allocator.alloc(u8, file_size);
    _ = try file.readAll(buffer);
    return buffer;
}
