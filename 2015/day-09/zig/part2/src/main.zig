const std = @import("std");
const mecha = @import("mecha");

pub fn main() !void {
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator();
    const input = try readFile(allocator, "../../input.txt");
    defer allocator.free(input);

    const lineDatas: []LineData = (try fileParser.parse(allocator, input)).value;

    var destinations = std.ArrayList([]const u8).init(allocator);
    defer destinations.deinit();

    for (lineDatas) |lineData| {
        if (!arrayListContains(destinations, lineData.from)) {
            try destinations.append(lineData.from);
        }

        if (!arrayListContains(destinations, lineData.to)) {
            try destinations.append(lineData.to);
        }
    }

    const numberOfPlaces = destinations.items.len;

    const matrix = try allocator.alloc(i32, numberOfPlaces * numberOfPlaces);
    defer allocator.free(matrix);

    for (0..matrix.len) |i| {
        matrix[i] = 0;
    }

    var destinationIdHashMap = std.StringHashMap(usize).init(allocator);
    defer destinationIdHashMap.deinit();
    for (0..destinations.items.len) |i| {
        try destinationIdHashMap.put(destinations.items[i], i);
    }

    for (lineDatas) |lineData| {
        const fromId = destinationIdHashMap.get(lineData.from).?;
        const toId = destinationIdHashMap.get(lineData.to).?;

        setMatrixValue(matrix, numberOfPlaces, fromId, toId, lineData.distance);
        setMatrixValue(matrix, numberOfPlaces, toId, fromId, lineData.distance);
    }

    var route = try allocator.alloc(usize, numberOfPlaces);
    defer allocator.free(route);

    for (0..route.len) |i| {
        route[i] = i;
    }

    var output = std.ArrayList(i32).init(allocator);
    defer output.deinit();

    try generatePermutations(numberOfPlaces, route, matrix, numberOfPlaces, &output);

    var max: i32 = 0;
    for (output.items) |item| {
        if (item > max) {
            max = item;
        }
    }

    std.debug.print("MAX DISTANCE: {d}\n", .{ max });

}

fn generatePermutations(k: usize, arr: []usize, matrix: []i32, numberOfPlaces: usize, output: *std.ArrayList(i32)) !void {
    if (k == 1) {
        const distance = getTotalDistance(arr, matrix, numberOfPlaces);
        try output.append(distance);
        return;
    }

    try generatePermutations(k - 1, arr, matrix, numberOfPlaces, output);

    for (0..k - 1) |i| {
        if (k % 2 == 0) {
            swap(arr, i, k - 1);
        } else {
            swap(arr, 0, k - 1);
        }

        try generatePermutations(k - 1, arr, matrix, numberOfPlaces, output);
    }
}

fn getTotalDistance(arr: []const usize, matrix: []i32, numberOfPlaces: usize) i32 {
    var distance: i32 = 0;

    for (0..arr.len - 1) |i| {
        distance += getMatrixValue(matrix, numberOfPlaces, arr[i], arr[i + 1]);
    }

    return distance;
}

fn swap(arr: []usize, idx1: usize, idx2: usize) void {
    const temp = arr[idx1];
    arr[idx1] = arr[idx2];
    arr[idx2] = temp;
}

fn anyDuplicateNumbers(allocator: std.mem.Allocator, route: []const usize) !bool {
    var track = try allocator.alloc(bool, route.len);
    defer allocator.free(track);

    for (0..track.len) |i| {
        track[i] = false;
    }

    for (route) |item| {
        if (track[item]) {
            return true;
        }

        track[item] = true;
    }

    return false;
}

fn getMatrixValue(matrix: []i32, numberOfPlaces: usize, row: usize, col: usize) i32 {
    return matrix[col * numberOfPlaces + row];
}

fn setMatrixValue(matrix: []i32, numberOfPlaces: usize, row: usize, col: usize, newVal: i32) void {
    matrix[col * numberOfPlaces + row] = newVal;
}

fn arrayListContains(arrayList: std.ArrayList([]const u8), element: []const u8) bool {
    for (arrayList.items) |item| {
        if (std.mem.eql(u8, item, element)) {
            return true;
        }
    }

    return false;
}

fn addEdgesBetweenNodes(node1: *Node, node2: *Node, weight: i32) !void {
    try node1.addEdge(node2, weight);
    try node2.addEdge(node1, weight);
}

fn readFile(allocator: std.mem.Allocator, fileName: []const u8) ![]u8 {
    const file = try std.fs.cwd().openFile(fileName, .{});
    defer file.close();

    const fileStats = try file.stat();
    const fileSize = fileStats.size;

    const buffer = try allocator.alloc(u8, fileSize);
    _ = try file.readAll(buffer);
    return buffer;
}

const Node = struct {
    allocator: std.mem.Allocator,
    neighbors: std.ArrayList(*Edge),
    name: []const u8,

    fn create(allocator: std.mem.Allocator, name: []const u8) !*Node {
        const node = try allocator.create(Node);

        node.allocator = allocator;
        node.neighbors = std.ArrayList(*Edge).init(allocator);
        node.name = try allocator.dupe(u8, name);

        return node;
    }

    fn free(self: *Node) void {
        for (self.neighbors.items) |edge| {
            self.allocator.destroy(edge);
        }

        self.neighbors.deinit();
        self.allocator.free(self.name);
        self.allocator.destroy(self);
    }

    fn addEdge(self: *Node, to: *Node, weight: i32) !void {
        const edge = try self.allocator.create(Edge);
        edge.node = to;
        edge.weight = weight;

        try self.neighbors.append(edge);
    }
};

const Edge = struct {
    weight: i32,
    node: *Node,
};

const LineData = struct {
    from: []const u8,
    to: []const u8,
    distance: i32,
};

const fileParser = mecha.many(
    lineParser,
    .{
        .separator = mecha.ascii.char('\n').discard()
    }
);

const lineParser = mecha.map(
    mecha.combine(.{
        mecha.many(mecha.ascii.alphabetic, .{ .min = 1 }),
        mecha.string(" to ").discard(),
        mecha.many(mecha.ascii.alphabetic, .{ .min = 1 }),
        mecha.string(" = ").discard(),
        mecha.int(i32, .{})
    }),
    mecha.toStruct(LineData)
);
