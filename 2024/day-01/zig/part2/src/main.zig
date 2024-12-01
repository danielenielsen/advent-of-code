const std = @import("std");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const input = try readFile("../../input.txt", allocator);
    defer allocator.free(input);

    var lines = std.mem.tokenizeAny(u8, input, "\n");

    var left_list = std.ArrayList(i32).init(allocator);
    var right_list = std.ArrayList(i32).init(allocator);

    while (lines.next()) |line| {
        if (std.mem.eql(u8, line, "")) {
            continue;
        }

        var split = std.mem.tokenizeAny(u8, line, "   ");
        const left_number = try std.fmt.parseInt(i32, split.next().?, 10);
        const right_number = try std.fmt.parseInt(i32, split.next().?, 10);

        try left_list.append(left_number);
        try right_list.append(right_number);
    }

    std.mem.sort(i32, left_list.items, {}, std.sort.asc(i32));
    std.mem.sort(i32, right_list.items, {}, std.sort.asc(i32));

    var i: usize = 0;
    var j: usize = 0;
    var result: i32 = 0;
    while (i < left_list.items.len) {
        while (j < right_list.items.len and left_list.items[i] > right_list.items[j]) {
            j += 1;
        }

        if (j >= right_list.items.len) {
            break;
        }

        if (left_list.items[i] != right_list.items[j]) {
            i += 1;
            continue;
        }

        var count: i32 = 0;
        while (left_list.items[i] == right_list.items[j]) {
            count += 1;
            j += 1;
        }

        result += left_list.items[i] * count;
        i += 1;
    }

    const writer = std.io.getStdOut().writer();
    try writer.print("Result: {d}\n", .{result});
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
