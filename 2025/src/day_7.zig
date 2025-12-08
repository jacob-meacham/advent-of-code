const std = @import("std");
const vector = @import("vector.zig");

const GridType = enum {
    start,
    empty,
    splitter
};

fn convert(char: u8, _: usize, _: usize) !GridType {
    return switch(char) {
        '.' => GridType.empty,
        '^' => GridType.splitter,
        'S' => GridType.start,
        else => return error.InvalidInput
    };
}

fn debugPrint(allocator: std.mem.Allocator, grid: *vector.Grid2D(GridType), splitPos: *std.AutoHashMap(vector.Pos2, void)) !void {
    const line = try allocator.alloc(u8, grid.width);
    defer allocator.free(line);

    for (0..grid.height) |y| {
        var index: usize = 0;
        for (0..grid.width) |x| {
            var char: u8 = switch (grid.get(x, y)) {
                GridType.start => 'S',
                GridType.splitter => '^',
                GridType.empty => '.'
            };
            if (splitPos.contains(vector.Pos2.fromU(x, y))) {
                if (char == '^') {
                    char = '^';
                } else {
                    char = '|';
                }
            }
            line[index] = char;
            index += 1;
        }
        std.debug.print("{s}\n", .{line});
    }
}

fn parseInput(allocator: std.mem.Allocator, input: []const u8) !vector.Grid2D(GridType) {
    var lines = std.mem.splitScalar(u8, input, '\n');
    var line_list = try std.ArrayList([]const u8).initCapacity(allocator, 150);
    defer line_list.deinit(allocator);

    while (lines.next()) |line| {
        if (line.len > 0) try line_list.append(allocator, line);
        _ = lines.next(); // Skip all empty lines
    }

    return try vector.Grid2D(GridType).createFromSlice(allocator, line_list.items, convert);
}

pub fn part1(allocator: std.mem.Allocator, input: []const u8) !i64 {
    var lines = std.mem.splitScalar(u8, input, '\n');
    var line_list = try std.ArrayList([]const u8).initCapacity(allocator, 150);
    defer line_list.deinit(allocator);

    while (lines.next()) |line| {
        if (line.len > 0) try line_list.append(allocator, line);
        _ = lines.next(); // Skip all empty lines
    }

    const grid = try vector.Grid2D(GridType).createFromSlice(allocator, line_list.items, convert);
    defer grid.deinit();

    var stack = try std.ArrayList(vector.Pos2).initCapacity(allocator, grid.width*grid.height);
    defer stack.deinit(allocator);

    var rays = std.AutoHashMap(vector.Pos2, void).init(allocator);
    defer rays.deinit();

    // Find the starting position
    for (0..grid.width) |x| {
        if (grid.get(x, 0) == GridType.start) {
            try stack.append(allocator, vector.Pos2.fromU(x, 1));
            break;
        }
    }

    var num_split: usize = 0;
    while (stack.items.len > 0) {
        const pos = stack.pop() orelse continue;
        if (rays.contains(pos)) continue;

        const x: isize = @intCast(pos.x);
        const y: isize = @intCast(pos.y);

        if (grid.getMaybe(x, y)) |grid_type| {
            switch (grid_type) {
                GridType.start => return error.InvalidInput,
                GridType.empty => {
                    try stack.append(allocator, vector.Pos2.from(x, y + 1));
                },
                GridType.splitter => {
                    try stack.append(allocator, vector.Pos2.from(x + 1, y + 1));
                    try stack.append(allocator, vector.Pos2.from(x - 1, y + 1));

                    //try debugPrint(allocator, &grid, &rays);

                    num_split += 1;
                }
            }

            try rays.put(pos, {});
        }
    }

    return @intCast(num_split);
}

fn splitLifetime(grid: *const vector.Grid2D(GridType), lifetimes: *std.AutoHashMap(vector.Pos2, usize), pos: vector.Pos2) !usize {
    if (lifetimes.get(pos)) |lifetime| return lifetime;

    const x: isize = @intCast(pos.x);
    const y: isize = @intCast(pos.y);

    if (grid.getMaybe(x, y)) |grid_type| {
        switch (grid_type) {
            GridType.start => return error.InvalidInput,
            GridType.empty => {
                const pos_lifetimes = try splitLifetime(grid, lifetimes, vector.Pos2.from(x, y + 1));
                try lifetimes.put(vector.Pos2.from(x, y), pos_lifetimes);

                return pos_lifetimes;
            },
            GridType.splitter => {
                const new1 = vector.Pos2.from(x + 1, y + 1);
                const new2 = vector.Pos2.from(x - 1, y + 1);

                const pos1_lifetimes = try splitLifetime(grid, lifetimes, new1);
                const pos2_lifetimes = try splitLifetime(grid, lifetimes, new2);
                try lifetimes.put(pos, pos1_lifetimes + pos2_lifetimes);

                return pos1_lifetimes + pos2_lifetimes;
            }
        }
    }

    // Made it to the bottom
    return 1;
}

pub fn part2(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const grid = try parseInput(allocator, input);
    defer grid.deinit();

    var lifetimes = std.AutoHashMap(vector.Pos2, usize).init(allocator);
    defer lifetimes.deinit();

    // Find the starting position
    for (0..grid.width) |x| {
        if (grid.get(x, 0) == GridType.start) {
            const t = try splitLifetime(&grid, &lifetimes, vector.Pos2.fromU(x, 1));
            return @intCast(t);
        }
    }

    return 0;
}
