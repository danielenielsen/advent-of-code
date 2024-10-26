const std = @import("std");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const input = try readFile(allocator, "../../input.txt");
    defer allocator.free(input);

    var lines = std.mem.split(u8, input, "\n");

    var totalChars: usize = 0;
    var expandedTotalChars: usize = 0;

    while (lines.next()) |line| {
        if (line.len == 0) {
            continue;
        }

        var arrayList = std.ArrayList(u8).init(allocator);
        defer arrayList.deinit();

        try arrayList.append('\"');
        for (line) |char| {
            if (char == '\\') {
                const slice = [2]u8 { '\\', '\\' };
                try arrayList.appendSlice(&slice);
            } else if (char == '\"') {
                const slice = [2]u8 { '\\', '\"' };
                try arrayList.appendSlice(&slice);
            } else {
                try arrayList.append(char);
            }
        }
        try arrayList.append('\"');

        totalChars += line.len;
        expandedTotalChars += arrayList.items.len;
    }

    std.debug.print("TOTAL: {d}, EXPANDED: {d}, DIFFERENCE: {d}\n", .{ totalChars, expandedTotalChars, expandedTotalChars - totalChars });
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
