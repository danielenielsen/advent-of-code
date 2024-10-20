const std = @import("std");
const fs = std.fs;

const mecha = @import("mecha");

const LineData = struct {
    game_number: i32,
    red_num: i32,
    green_num: i32,
    blue_num: i32,
};

pub fn main() !void {
    const file_contents = try readFile("../../input.txt", std.heap.page_allocator);
    var line_iter = std.mem.tokenizeAny(u8, file_contents, "\r\n");

    var sum: i32 = 0;
    line_loop: while (line_iter.next()) |line| {
        const parsed_line = (try line_parser.parse(std.heap.page_allocator, line)).value;
        const game_num: i32 = parsed_line[0];
        const rounds: [][]Roll = parsed_line[1];

        for (rounds) |round| {
            for (round) |roll| {
                const color_limit: i32 = switch (roll.color) {
                    Color.red => 12,
                    Color.green => 13,
                    Color.blue => 14,
                };

                if (roll.number > color_limit) {
                    continue :line_loop;
                }
            }
        }

        sum += game_num;
    }

    std.debug.print("Sum: {}\n", .{ sum });
}

const Color = enum {
    red,
    green,
    blue,
};

const Roll = struct {
    number: i32,
    color: Color,
};

const header_parser = mecha.combine(.{
        mecha.string("Game").discard(),
        mecha.many(mecha.ascii.whitespace, .{}).discard(),
        mecha.int(i32, .{ .base = 10 }),
        mecha.ascii.char(':').discard(),
        mecha.many(mecha.ascii.whitespace, .{}).discard(),
    });

const inner_body_parser = mecha.many(
    mecha.map(
        mecha.combine(.{
        mecha.many(mecha.ascii.whitespace, .{}).discard(),
        mecha.int(i32, .{ .base = 10 }),
        mecha.many(mecha.ascii.whitespace, .{}).discard(),
        mecha.enumeration(Color)
    }), mecha.toStruct(Roll)),
    .{
    .separator = mecha.ascii.char(',').discard(),
    .min = 0,
    .max = 100,
    .collect = true
});

const body_parser = mecha.many(
    inner_body_parser,
    .{
        .separator = mecha.ascii.char(';').discard() 
    }
);

const line_parser = mecha.combine(.{
    header_parser,
    body_parser,
});

fn readFile(file_name: []const u8, allocator: std.mem.Allocator) ![]u8 {
    const file = try fs.cwd().openFile(file_name, .{});
    defer file.close();

    const file_stats = try file.stat();
    const file_size = file_stats.size;
    const buffer = try allocator.alloc(u8, file_size);
    _ = try file.readAll(buffer);
    return buffer;
}
