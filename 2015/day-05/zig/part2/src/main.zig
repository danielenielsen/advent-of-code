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
    var has_skipping_pair = false;
    var has_repeating_pair = false;

    var prev_char: ?u8 = null;
    var prev_prev_char: ?u8 = null;

    for (string, 0..) |char, idx| {

        if (prev_prev_char) |value| {
            if (value == char) {
                has_skipping_pair = true;
            }
        }

        if (prev_char) |value| {

            if (!has_repeating_pair and idx < string.len - 2) {
                const pair = [2]u8 { value, char };

                if (doesStringContain(string[(idx + 1)..], pair)) {
                    has_repeating_pair = true;
                }
            }

            prev_prev_char = value;
        }

        prev_char = char;
    }

    return has_skipping_pair and has_repeating_pair;
}

fn doesStringContain(string: []const u8, value: [2]u8) bool {
    if (string.len < 2) {
        return false;
    }

    for (0..string.len - 1) |i| {
        if (value[0] == string[i] and value[1] == string[i + 1]) {
            return true;
        }
    }

    return false;
}