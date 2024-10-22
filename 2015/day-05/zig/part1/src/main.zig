const std = @import("std");

pub fn main() !void {
    var gba = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gba.allocator();
    const input = try readFile("../../input.txt", allocator);
    defer allocator.free(input);

    var split = std.mem.splitSequence(u8, input, "\n");
    var count: i32 = 0;

    while (split.next()) |line| {
        if (isStringNice(line)) {
            count += 1;
        }
    }

    const stdout = std.io.getStdOut().writer();
    try stdout.print("NICE COUNT: {d}\n", .{ count });
}

fn readFile(file_name: []const u8, allocator: std.mem.Allocator) ![]u8 {
    const file = try std.fs.cwd().openFile(file_name, .{});
    defer file.close();

    const stats = try file.stat();
    const size = stats.size;
    const buffer = try allocator.alloc(u8, size);
    _ = try file.readAll(buffer);
    return buffer;
}

fn isStringNice(string: []const u8) bool {
    var vowel_count: i32 = 0;
    var double_letter = false;
    var has_invalid_combo = false;
    var prev_char: ?u8 = null;

    for (string) |char| {
        if (isVowel(char)) {
            vowel_count += 1;
        }

        if (prev_char) |value| {
            if (value == char) {
                double_letter = true;
            }

            if (value == 'a' and char == 'b') {
                has_invalid_combo = true;
            }

            if (value == 'c' and char == 'd') {
                has_invalid_combo = true;
            }

            if (value == 'p' and char == 'q') {
                has_invalid_combo = true;
            }

            if (value == 'x' and char == 'y') {
                has_invalid_combo = true;
            }
        }

        prev_char = char;
    }

    return vowel_count > 2 and double_letter and !has_invalid_combo;
}

fn isVowel(char: u8) bool {
    return char == 'a' or char == 'e' or char == 'i' or char == 'o' or char == 'u';
}
