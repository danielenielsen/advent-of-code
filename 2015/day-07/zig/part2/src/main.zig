const std = @import("std");
const fileHandler = @import("./fileHandler.zig");
const LineType = fileHandler.LineType;
const Value = fileHandler.Value;

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const parsedLines: []LineType = try fileHandler.getLines("../../input.txt", allocator);
    // How to deinit parsedLines?

    var hashMap = std.StringHashMap(i32).init(allocator);

    try findA(allocator, parsedLines, &hashMap);

    const firstA = hashMap.get("a").?;
    hashMap.clearAndFree();
    hashMap = std.StringHashMap(i32).init(allocator);
    try hashMap.put("b", firstA);
    defer hashMap.deinit();

    std.debug.print(" --- RESET --- \n", .{});
    std.debug.print("INSERT {d} AS b\n", .{ firstA });

    try findA(allocator, parsedLines, &hashMap);

    const a = hashMap.get("a").?;
    std.debug.print("A: {d}\n", .{ a });
}

fn findA(allocator: std.mem.Allocator, parsedLines: []const LineType, hashMap: *std.StringHashMap(i32)) !void {
    while (!hashMap.contains("a")) {
        for (parsedLines) |line| {
            if (shouldSkipLine(line, hashMap)) {
                continue;
            }

            switch (line) {
                .assign => |value| {
                    const valueContent = try getValueContent(allocator, hashMap, value.in) orelse continue;
                    defer valueContent.free();

                    std.debug.print("ASSIGN: {s} = {s}\n", .{ value.out, valueContent.string });
                    try hashMap.put(value.out, valueContent.num);
                },
                .bitand => |value| {
                    const valueContentLeft = try getValueContent(allocator, hashMap, value.left) orelse continue;
                    defer valueContentLeft.free();

                    const valueContentRight = try getValueContent(allocator, hashMap, value.right) orelse continue;
                    defer valueContentRight.free();

                    const res = valueContentLeft.num & valueContentRight.num;
                    std.debug.print("AND: {s} = {s} & {s} = {d}\n", .{ value.out, valueContentLeft.string, valueContentRight.string, res });
                    try hashMap.put(value.out, res);
                },
                .bitor => |value| {
                    const valueContentLeft = try getValueContent(allocator, hashMap, value.left) orelse continue;
                    defer valueContentLeft.free();

                    const valueContentRight = try getValueContent(allocator, hashMap, value.right) orelse continue;
                    defer valueContentRight.free();

                    const res = valueContentLeft.num | valueContentRight.num;
                    std.debug.print("OR: {s} = {s} | {s} = {d}\n", .{ value.out, valueContentLeft.string, valueContentRight.string, res });
                    try hashMap.put(value.out, res);
                },
                .bitnot => |value| {
                    const valueContent = try getValueContent(allocator, hashMap, value.in) orelse continue;
                    defer valueContent.free();

                    const res = ~valueContent.num;
                    std.debug.print("NOT: {s} = ~{s} = {d}\n", .{ value.out, valueContent.string, res });
                    try hashMap.put(value.out, res);
                },
                .leftshift => |value| {
                    const valueContent = try getValueContent(allocator, hashMap, value.in) orelse continue;
                    defer valueContent.free();

                    const res = valueContent.num << value.num;
                    std.debug.print("LSHIFT: {s} = {s} << {d} = {d}\n", .{ value.out, valueContent.string, value.num, res });
                    try hashMap.put(value.out, res);
                },
                .rightshift => |value| {
                    const valueContent = try getValueContent(allocator, hashMap, value.in) orelse continue;
                    defer valueContent.free();

                    const res = valueContent.num >> value.num;
                    std.debug.print("RSHIFT: {s} = {s} >> {d} = {d}\n", .{ value.out, valueContent.string, value.num, res });
                    try hashMap.put(value.out, res);
                },
            }
        }
    }
}


fn shouldSkipLine(lineType: LineType, hashMap: *std.StringHashMap(i32)) bool {
    switch (lineType) {
        .assign => |value| {
            if (hashMap.contains(value.out)) {
                return true;
            }
        },
        .bitand => |value| {
            if (hashMap.contains(value.out)) {
                return true;
            }
        },
        .bitor => |value| {
            if (hashMap.contains(value.out)) {
                return true;
            }
        },
        .bitnot => |value| {
            if (hashMap.contains(value.out)) {
                return true;
            }
        },
        .leftshift => |value| {
            if (hashMap.contains(value.out)) {
                return true;
            }
        },
        .rightshift => |value| {
            if (hashMap.contains(value.out)) {
                return true;
            }
        },
    }

    return false;
}

const ValueContent = struct {
    allocator: std.mem.Allocator,
    num: i32,
    string: []const u8,

    fn free(self: *const ValueContent) void {
        self.allocator.free(self.string);
    }
};

fn getValueContent(allocator: std.mem.Allocator, hashMap: *std.StringHashMap(i32), value: Value) !?ValueContent {
    switch (value) {
        .constant => |val| {
            return .{
                .allocator = allocator,
                .num = val,
                .string = try std.fmt.allocPrint(allocator, "{d}", .{ val })
            };
        },
        .identifier => |name| {
            if (!hashMap.contains(name)) {
                return null;
            }

            const val = hashMap.get(name).?;

            return .{
                .allocator = allocator,
                .num = val,
                .string = try std.fmt.allocPrint(allocator, "{s}({d})", .{ name, val }),
            };
        }
    }
}
