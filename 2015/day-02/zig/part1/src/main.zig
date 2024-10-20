const std = @import("std");
const mecha = @import("mecha");

const LineData = struct {
    length: i32,
    width: i32,
    height: i32,
};

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const input = try readFile("../../input.txt", allocator);
    defer allocator.free(input);

    const stdout_file = std.io.getStdOut().writer();
    var bw = std.io.bufferedWriter(stdout_file);
    const stdout = bw.writer();

    try stdout.print("Input has a length of {d}\n", .{ input.len });

    const data = (try file_parser.parse(allocator, input)).value;

    var sum: i32 = 0;
    for (data) |linedata| { 
        sum += handleLine(&linedata);
    }

    try stdout.print("SUM: {d}\n", .{ sum });
    try bw.flush();
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

fn handleLine(linedata: *const LineData) i32 {
    const surface_area = 2 * linedata.length * linedata.width + 2 * linedata.width * linedata.height + 2 * linedata.height * linedata.length;
    const slack = @min(linedata.length * linedata.width, @min(linedata.width * linedata.height, linedata.length * linedata.height));

    return surface_area + slack;
}

const file_parser = mecha.many(
    line_parser,
    .{
        .separator = newline_parser
    }
);

const line_parser = mecha.map(
    mecha.combine(.{
        mecha.int(i32, .{ .base = 10 }),
        mecha.ascii.char('x').discard(),
        mecha.int(i32, .{ .base = 10 }),
        mecha.ascii.char('x').discard(),
        mecha.int(i32, .{ .base = 10 }),
    }),
    mecha.toStruct(LineData)
);

const newline_parser = mecha.oneOf(.{
    mecha.string("\r\n").discard(),
    mecha.ascii.char('\n').discard(),
});
