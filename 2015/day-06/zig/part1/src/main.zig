const std = @import("std");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const input = try readFile("../../input.txt", allocator);
    defer allocator.free(input);

    var grid: [1000][1000]bool = undefined;

    for (0..grid.len) |i| {
        for (0..grid[i].len) |j| {
            grid[i][j] = false;
        }
    }

    var lines = std.mem.split(u8, input, "\n");
    while (lines.next()) |line| {
        if (line.len == 0) {
            continue;
        }

        const line_data = try parseLine(line);
        handleLine(&line_data, &grid);
    }

    var count: i32 = 0;
    for (0..grid.len) |i| {
        for (0..grid[i].len) |j| {
            if (grid[i][j]) {
                count += 1;
            }
        }
    }

    std.debug.print("COUNT: {d}\n", .{ count });
}

fn handleLine(line_data: *const LineData, grid: *[1000][1000]bool) void {
    for (line_data.from.x..(line_data.to.x + 1)) |i| {
        for (line_data.from.y..(line_data.to.y + 1)) |j| {
            if (line_data.line_type == LineType.Toggle) {
                grid[i][j] = !grid[i][j];
            } else if (line_data.line_type == LineType.TurnOn) {
                grid[i][j] = true;
            } else if (line_data.line_type == LineType.TurnOff) {
                grid[i][j] = false;
            }
        }
    }
}

fn readFile(fileName: []const u8, allocator: std.mem.Allocator) ![]u8 {
    const file = try std.fs.cwd().openFile(fileName, .{});
    defer file.close();

    const file_stat = try file.stat();
    const file_size = file_stat.size;
    const buffer = try allocator.alloc(u8, file_size);

    _ = try file.readAll(buffer);
    return buffer;
}

fn parseLine(line_string: []const u8) !LineData {
    var string = line_string;
    var line_type: ?LineType = null;

    if (std.mem.eql(u8, string[0..6], "toggle")) {
        line_type = LineType.Toggle;
        string = string[7..];
    } else if (std.mem.eql(u8, string[0..7], "turn on")) {
        line_type = LineType.TurnOn;
        string = string[8..];
    } else if (std.mem.eql(u8, string[0..8], "turn off")) {
        line_type = LineType.TurnOff;
        string = string[9..];
    } else {
        return error.CouldNotParseType;
    }

    const next_space_index = findNextSpace(string).?;
    const from_string = string[0..next_space_index];
    const to_string = string[(next_space_index + 9)..];

    var from_string_iter = std.mem.split(u8, from_string, ",");
    var to_string_iter = std.mem.split(u8, to_string, ",");

    const from_coords: CoordTuple = .{
        .x = try std.fmt.parseInt(usize, from_string_iter.next().?, 10),
        .y = try std.fmt.parseInt(usize, from_string_iter.next().?, 10),
    };

    const to_coords: CoordTuple = .{
        .x = try std.fmt.parseInt(usize, to_string_iter.next().?, 10),
        .y = try std.fmt.parseInt(usize, to_string_iter.next().?, 10),
    };

    return .{
        .line_type = line_type.?,
        .from = from_coords,
        .to = to_coords,
    };
}

fn findNextSpace(string: []const u8) ?usize {
    for (string, 0..) |char, idx| {
        if (char == ' ') {
            return idx;
        }
    }

    return null;
}

const LineData = struct {
    line_type: LineType,
    from: CoordTuple,
    to: CoordTuple,
};

const LineType = enum {
    TurnOn,
    TurnOff,
    Toggle,
};

const CoordTuple = struct {
    x: usize,
    y: usize,
};
