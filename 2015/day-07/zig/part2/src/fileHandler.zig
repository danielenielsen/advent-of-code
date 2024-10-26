const std = @import("std");
const mecha = @import("mecha");

pub fn getLines(fileName: []const u8, allocator: std.mem.Allocator) ![]LineType {
    const input = try readFile(fileName, allocator);
    const lines: []LineType = (try parseFile.parse(allocator, input)).value;
    return lines;
}

fn readFile(fileName: []const u8, allocator: std.mem.Allocator) ![]u8 {
    const file = try std.fs.cwd().openFile(fileName, .{});
    defer file.close();

    const fileStats = try file.stat();
    const fileSize = fileStats.size;
    const buffer = try allocator.alloc(u8, fileSize);
    _ = try file.readAll(buffer);

    return buffer;
}

pub const LineType = union(enum) {
    assign: AssignLine,
    bitand: BitwiseAndLine,
    bitor: BitwiseOrLine,
    bitnot: BitwiseNotLine,
    leftshift: LeftShiftLine,
    rightshift: RightShiftLine,
};

pub const Value = union(enum) {
    constant: i32,
    identifier: []const u8,
};

const AssignLine = struct {
    in: Value,
    out: []const u8,
};

const BitwiseAndLine = struct {
    left: Value,
    right: Value,
    out: []const u8,
};

const BitwiseOrLine = struct {
    left: Value,
    right: Value,
    out: []const u8,
};

const BitwiseNotLine = struct {
    in: Value,
    out: []const u8,
};

const LeftShiftLine = struct {
    in: Value,
    num: u5,
    out: []const u8,
};

const RightShiftLine = struct {
    in: Value,
    num: u5,
    out: []const u8,
};

const parseFile = mecha.many(parseLine, .{
    .separator = mecha.ascii.char('\n').discard()
});

const parseLine = mecha.oneOf(.{
    mecha.convert(parseAssign, assignConverter),
    mecha.convert(parseAnd, andConverter),
    mecha.convert(parseOr, orConverter),
    mecha.convert(parseNot, notConverter),
    mecha.convert(parseLeftShift, leftShiftConverter),
    mecha.convert(parseRightShift, rightShiftConverter),
});

const parseConstant = mecha.int(i32, .{});
const parseIdentifier = mecha.many(mecha.ascii.alphabetic, .{ .min = 1 });
const parseValue = mecha.oneOf(.{
    mecha.convert(parseConstant, constantConverter),
    mecha.convert(parseIdentifier, identifierConverter),
});

const parseAssign = mecha.map(mecha.combine(.{
        parseValue,
        mecha.string(" -> ").discard(),
        mecha.many(mecha.ascii.alphabetic, .{ .min = 1 }),
    }),
    mecha.toStruct(AssignLine)
);

const parseAnd = mecha.map(mecha.combine(.{
        parseValue,
        mecha.string(" AND ").discard(),
        parseValue,
        mecha.string(" -> ").discard(),
        mecha.many(mecha.ascii.alphabetic, .{ .min = 1 }),
    }),
    mecha.toStruct(BitwiseAndLine)
);

const parseOr = mecha.map(mecha.combine(.{
        parseValue,
        mecha.string(" OR ").discard(),
        parseValue,
        mecha.string(" -> ").discard(),
        mecha.many(mecha.ascii.alphabetic, .{ .min = 1 }),
    }),
    mecha.toStruct(BitwiseOrLine)
);

const parseNot = mecha.map(mecha.combine(.{
        mecha.string("NOT ").discard(),
        parseValue,
        mecha.string(" -> ").discard(),
        mecha.many(mecha.ascii.alphabetic, .{ .min = 1 }),
    }),
    mecha.toStruct(BitwiseNotLine)
);

const parseLeftShift = mecha.map(mecha.combine(.{
        parseValue,
        mecha.string(" LSHIFT ").discard(),
        mecha.int(u5, .{ .parse_sign = false }),
        mecha.string(" -> ").discard(),
        mecha.many(mecha.ascii.alphabetic, .{ .min = 1 }),
    }),
    mecha.toStruct(LeftShiftLine)
);

const parseRightShift = mecha.map(mecha.combine(.{
        parseValue,
        mecha.string(" RSHIFT ").discard(),
        mecha.int(u5, .{ .parse_sign = false }),
        mecha.string(" -> ").discard(),
        mecha.many(mecha.ascii.alphabetic, .{ .min = 1 }),
    }),
    mecha.toStruct(RightShiftLine)
);

const constantConverter = struct {
    fn func(_: std.mem.Allocator, value: i32) !Value {
        return Value {
            .constant = value,
        };
    }
}.func;

const identifierConverter = struct {
    fn func(_: std.mem.Allocator, value: []const u8) !Value {
        return Value {
            .identifier = value,
        };
    }
}.func;

const assignConverter = struct {
    fn func(_: std.mem.Allocator, value: AssignLine) !LineType {
        return LineType {
            .assign = value,
        };
    }
}.func;

const andConverter = struct {
    fn func(_: std.mem.Allocator, value: BitwiseAndLine) !LineType {
        return LineType {
            .bitand = value,
        };
    }
}.func;

const orConverter = struct {
    fn func(_: std.mem.Allocator, value: BitwiseOrLine) !LineType {
        return LineType {
            .bitor = value,
        };
    }
}.func;

const notConverter = struct {
    fn func(_: std.mem.Allocator, value: BitwiseNotLine) !LineType {
        return LineType {
            .bitnot = value,
        };
    }
}.func;

const leftShiftConverter = struct {
    fn func(_: std.mem.Allocator, value: LeftShiftLine) !LineType {
        return LineType {
            .leftshift = value,
        };
    }
}.func;

const rightShiftConverter = struct {
    fn func(_: std.mem.Allocator, value: RightShiftLine) !LineType {
        return LineType {
            .rightshift = value,
        };
    }
}.func;

