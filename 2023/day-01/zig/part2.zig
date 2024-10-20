const std = @import("std");
const fs = std.fs;

pub fn main() !void {
    const file_name = "../input.txt";
    const buffer = try readFile(file_name, std.heap.page_allocator);

    const character_numbers = [_]*const []const u8{
        &"zero",
        &"one",
        &"two",
        &"three",
        &"four",
        &"five",
        &"six",
        &"seven",
        &"eight",
        &"nine",
    };

    var first_char: ?u8 = null;
    var second_char: ?u8 = null;
    var sum: u64 = 0;

    for (buffer, 0..) |char, index| {
        if (char == '\n') {
            if (second_char != null) {
                sum += asciiDigitToDigit(first_char.?) * 10 + asciiDigitToDigit(second_char.?);
            } else {
                sum += asciiDigitToDigit(first_char.?) * 10 + asciiDigitToDigit(first_char.?);
            }

            first_char = null;
            second_char = null;
            continue;
        }

        if (isNumberAtIndex(buffer, index, &character_numbers)) |val| {
            if (first_char == null) {
                first_char = val;
            } else {
                second_char = val;
            }
        }
    }

    std.debug.print("Sum: {}\n", .{sum});
}

fn isNumberAtIndex(buffer: []u8, buffer_index: usize, character_numbers: []const *const []const u8) ?u8 {
    const char_at_pos = buffer[buffer_index];

    if (char_at_pos == '\n') {
        return null;
    }

    if (isDigit(char_at_pos)) {
        return char_at_pos;
    }

    outer: for (character_numbers, 0..) |character_number, index1| {
        for (character_number.*, 0..) |char, index2| {
            if (buffer.len <= buffer_index + index2) {
                continue :outer;
            }

            if (buffer[buffer_index + index2] == '\n') {
                continue :outer;
            }

            if (buffer[buffer_index + index2] != char) {
                continue :outer;
            }
        }

        return @intCast(index1 + 48);
    }

    return null;
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
