const std = @import("std");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const input = try readFile("../../input.txt", allocator);
    defer allocator.free(input);

    var buffer: [16]u8 = undefined;
    var num: i32 = 0;
    
    while (true) : (num += 1) {
        const data_to_be_hashed = try std.fmt.allocPrint(allocator, "{s}{d}", .{ input, num });
        defer allocator.free(data_to_be_hashed);
        std.crypto.hash.Md5.hash(data_to_be_hashed, &buffer, .{});

        if (checkHash(&buffer)) {
            break;
        }
    }

    std.debug.print("NUMBER: {d}\n", .{ num });
}

fn checkHash(hash: *const [16]u8) bool {
    return hash[0] == 0 and hash[1] == 0 and hash[2] == 0;
}

fn readFile(file_name: []const u8, allocator: std.mem.Allocator) ![]u8 {
    const file = try std.fs.cwd().openFile(file_name, .{});
    defer file.close();

    const file_stats = try file.stat();
    const file_size = file_stats.size;
    const buffer = try allocator.alloc(u8, file_size);
    _ = try file.readAll(buffer);
    return buffer;
}
