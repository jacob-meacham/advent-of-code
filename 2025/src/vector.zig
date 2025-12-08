const std = @import("std");

pub const DIRECTIONS_8 = [_][2]i32{
    .{ -1, 0 },  // up
    .{ 1, 0 },   // down
    .{ 0, -1 },  // left
    .{ 0, 1 },   // right
    .{ -1, -1 }, // up-left
    .{ -1, 1 },  // up-right
    .{ 1, -1 },  // down-left
    .{ 1, 1 },   // down-right
};

pub const Pos2 = struct {
    const Self = @This();

    x: isize,
    y: isize,

    pub fn from(x: isize, y: isize) Self {
        return .{
            .x = x,
            .y = y
        };
    }

    pub fn fromU(ux: usize, uy: usize) Self {
        const x: isize = @intCast(ux);
        const y: isize = @intCast(uy);

        return .{
            .x = x,
            .y = y
        };
    }
};

pub fn Grid2D(comptime T: type) type {
    return struct {
        const Self = @This();

        cells: []T,
        width: usize,
        height: usize,
        allocator: std.mem.Allocator,

        pub fn init(allocator: std.mem.Allocator, width: usize, height: usize) !Self {
            const cells = try allocator.alloc(T, width * height);
            return Self{
                .cells = cells,
                .width = width,
                .height = height,
                .allocator = allocator,
            };
        }

        pub fn deinit(self: *const Self) void {
            self.allocator.free(self.cells);
        }

        pub fn createFromInput(
            allocator: std.mem.Allocator,
            input: []const u8,
            comptime convertFn: fn (char: u8, x: usize, y: usize) anyerror!T
        ) !Self {
            var lines = std.mem.splitScalar(u8, input, '\n');
            var line_list = try std.ArrayList([]const u8).initCapacity(allocator, 150);
            defer line_list.deinit(allocator);

            while (lines.next()) |line| {
                if (line.len > 0) try line_list.append(allocator, line);
            }

            return createFromSlice(allocator, line_list.items, convertFn);
        }

        pub fn createFromSlice(
            allocator: std.mem.Allocator,
            lines: [][] const u8,
            comptime convertFn: fn (char: u8, x: usize, y: usize) anyerror!T
        ) !Self {
            const height = lines.len;
            if (height == 0) return error.EmptyInput;
            const width = lines[0].len;

            var grid = try Self.init(allocator, width, height);
            errdefer grid.deinit();

            for (lines, 0..) |line, y| {
                for (line, 0..) |char, x| {
                    grid.set(x, y, try convertFn(char, x, y));
                }
            }

            return grid;
        }

        pub fn get(self: *const Self, x: usize, y: usize) T {
            return self.cells[y * self.width + x];
        }

        pub fn getMaybe(self: *const Self, x: isize, y: isize) ?T {
            if (!self.contains(x, y)) {
                return null;
            }

            const ux: usize = @intCast(x);
            const uy: usize = @intCast(y);

            return self.cells[uy * self.width + ux];
        }

        pub fn getPtr(self: *Self, x: usize, y: usize) *T {
            return &self.cells[y * self.width + x];
        }

        pub fn set(self: *Self, x: usize, y: usize, value: T) void {
            self.cells[y * self.width + x] = value;
        }

        pub fn contains(self: *const Self, x: isize, y: isize) bool {
            return x >= 0 and x < self.width and y >= 0 and y < self.height;
        }

        pub const CellIterator = struct {
            grid: *const Self,
            index: usize = 0,

            pub fn next(self: *CellIterator) ?struct { T, usize, usize } {
                if (self.index >= self.grid.cells.len) return null;

                const y = self.index / self.grid.width;
                const x = self.index % self.grid.width;
                const cell = self.grid.cells[self.index];
                self.index += 1;

                return .{ cell, x, y };
            }
        };

        pub fn iterator(self: *const Self) CellIterator {
            return CellIterator{ .grid = self };
        }
    };
}