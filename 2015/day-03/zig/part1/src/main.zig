const std = @import("std");
const mecha = @import("mecha");
const zigset = @import("ziglangSet");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const input = try readFile("../../input.txt", allocator);
    defer allocator.free(input);

    const data: []Direction = (try parser.parse(allocator, input)).value;

    var set = zigset.Set(struct {i32, i32}).init(allocator);
    defer set.deinit();

    _ = try set.add(.{ 0, 0 });

    var x: i32 = 0;
    var y: i32 = 0;

    for (data) |direction| {
        switch (direction) {
            Direction.North => y += 1,
            Direction.East => x += 1,
            Direction.South => y -= 1,
            Direction.West => x -= 1,
        }

        _ = try set.add(.{ x, y });
    }

    std.debug.print("NUMBER OF HOUSES: {d}\n", .{ set.cardinality() });
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

const Direction = enum {
    North,
    East,
    South,
    West
};

const parser = mecha.many(direction_parser, .{});

const direction_parser = mecha.oneOf(.{
    mecha.mapConst(mecha.ascii.char('^'), Direction.North),
    mecha.mapConst(mecha.ascii.char('>'), Direction.East),
    mecha.mapConst(mecha.ascii.char('v'), Direction.South),
    mecha.mapConst(mecha.ascii.char('<'), Direction.West),
});
