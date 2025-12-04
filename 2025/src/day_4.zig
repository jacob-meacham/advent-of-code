const std = @import("std");
const vector = @import("vector.zig");

fn convert(char: u8, _: usize, _: usize) !u8 {
    return switch(char) {
        '.' => 0,
        '@' => 1,
        else => return error.InvalidInput
    };
}


pub fn part1(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const grid = try vector.Grid2D(u8).createFromInput(allocator, input, convert);
    defer grid.deinit();

    var num_accessible: usize = 0;
    var it = grid.iterator();
    while (it.next()) |item| {
        const cell, const ux, const uy = item;

        const x: isize = @intCast(ux);
        const y: isize = @intCast(uy);

        if (cell == 0) {
            continue;
        }

        var num_neighbors: usize = 0;
        for (vector.DIRECTIONS_8) |dir| {
            const dx, const dy = dir;
            num_neighbors += grid.getMaybe(x + dx, y + dy) orelse 0;
        }

        if (num_neighbors < 4) {
            num_accessible += 1;
        }
    }

    return @intCast(num_accessible);
}

fn countNeighbors(grid: * const vector.Grid2D(u8), ux: usize, uy: usize) usize {
    const x: isize = @intCast(ux);
    const y: isize = @intCast(uy);

    var num_neighbors: usize = 0;
    for (vector.DIRECTIONS_8) |dir| {
        const dx, const dy = dir;
        num_neighbors += grid.getMaybe(x + dx, y + dy) orelse 0;
    }

    return num_neighbors;
}

pub fn part2(allocator: std.mem.Allocator, input: []const u8) !i64 {
    var grid = try vector.Grid2D(u8).createFromInput(allocator, input, convert);
    defer grid.deinit();

    var stack = try std.ArrayList(struct { usize, usize }).initCapacity(allocator, grid.width*grid.height);
    defer stack.deinit(allocator);

    var it = grid.iterator();
    while (it.next()) |item| {
        const cell, const x, const y = item;
        if (cell != 0) {
            try stack.append(allocator, .{x, y});
        }
    }

    var removed: i64 = 0;

    while (stack.items.len > 0) {
        const pos = stack.pop() orelse continue;
        const ux, const uy = pos;
        const x: isize = @intCast(ux);
        const y: isize = @intCast(uy);

        if (grid.get(ux, uy) == 0) {
            continue; // Already removed from previous
        }

        const neighbors = countNeighbors(&grid, ux, uy);
        if (neighbors < 4) {
            // Accessible now, so remove it
            removed += 1;
            grid.set(ux, uy, 0);

            // Re-check all occupied surrounding neighbors
            for (vector.DIRECTIONS_8) |dir| {
                const dx, const dy = dir;
                const cell = grid.getMaybe(x + dx, y + dy) orelse continue;
                if (cell > 0) {
                    const new_x = @as(usize, @intCast(x + dx));
                    const new_y = @as(usize, @intCast(y + dy));
                    try stack.append(allocator, .{ new_x, new_y });
                }
            }
        }
    }

    return removed;
}
