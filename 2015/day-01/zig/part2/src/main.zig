const std = @import("std");

pub fn main() !void {
    const file = try std.fs.cwd().openFile("../../input.txt", .{});
    defer file.close();

    var buf_reader = std.io.bufferedReader(file.reader());
    var in_stream = buf_reader.reader();

    var buf: [10000]u8 = undefined;
    const bytes_read = try in_stream.readAll(&buf);

    const stdout = std.io.getStdOut().writer();
    try stdout.print("Read {d} bytes\n", .{ bytes_read });

    const string = buf[0..bytes_read];

    var level: i32 = 0;
    var basement_pos: usize = undefined;
    for (string, 0..) |char, idx| {
        if (char == '(') {
            level += 1;
        } else {{
            level -= 1;
        }}

        if (level == -1) {
            basement_pos = idx + 1;
            break;
        }
    }

    try stdout.print("The first position in the basement is {d}\n", .{ basement_pos });
}

