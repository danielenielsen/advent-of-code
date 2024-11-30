const std = @import("std");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const input =  try readFile("../../input.txt", allocator);
    defer allocator.free(input);

    const writer = std.io.getStdOut().writer();
    try writer.print("{s}\n", .{ input });
}

fn readFile(file_path: []const u8, allocator: std.mem.Allocator) ![]u8 {
    const file = try std.fs.cwd().openFile(file_path, .{});
    defer file.close();

    const stats = try file.stat();
    const size = stats.size;

    const buffer = try allocator.alloc(u8, size);
    _ = try file.readAll(buffer);

    return buffer;
} 
