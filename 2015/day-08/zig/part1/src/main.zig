const std = @import("std");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const input = try readFile(allocator, "../../input.txt");
    defer allocator.free(input);

    var lines = std.mem.split(u8, input, "\n");

    var totalChars: usize = 0;
    var memoryChars: usize = 0;

    while (lines.next()) |line| {
        if (line.len == 0) {
            continue;
        }

        totalChars += line.len;

        var i: usize = 1;
        while (i < line.len - 1) {
            if (line[i] == '\\' and line[i + 1] == 'x') {
                memoryChars += 1;
                i += 4;
            } else if (line[i] == '\\') {
                memoryChars += 1;
                i += 2;
            } else {
                memoryChars += 1;
                i += 1;
            }
        }
    }

    std.debug.print("TOTAL: {d}, MEMORY: {d}, DIFFERENCE: {d}\n", .{ totalChars, memoryChars, totalChars - memoryChars });
}

fn readFile(allocator: std.mem.Allocator, fileName: []const u8) ![]u8 {
    const file = try std.fs.cwd().openFile(fileName, .{});
    defer file.close();

    const fileStats = try file.stat();
    const fileSize = fileStats.size;

    const buffer = try allocator.alloc(u8, fileSize);
    _ = try file.readAll(buffer);

    return buffer;
}
