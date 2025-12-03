const std = @import("std");

const Record = struct {
    direction: u8,
    num: i32,

    pub fn parse(line: []const u8) !Record {
        const rest = line[1..];

        const num = try std.fmt.parseInt(i32, rest, 10);

        return .{
            .direction = line[0],
            .num = num
        };
    }
};

fn parseInput(allocator: std.mem.Allocator, input: []const u8) ![]const Record {
    var records = try std.ArrayList(Record).initCapacity(allocator, 5000);
    errdefer records.deinit(allocator);

    var reader: std.io.Reader = .fixed(input);
    while (true) {
        const mline = try reader.takeDelimiter('\n');
        if (mline) |line| {
            const rec = try Record.parse(line);
            try records.append(allocator, rec);
        } else break;
    }


    return records.toOwnedSlice(allocator);
}

pub fn part1(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const records = try parseInput(allocator, input);
    defer allocator.free(records);

    var dial: i32 = 50;
    var num_zero: u32 = 0;
    for (records) |rec| {
        switch (rec.direction) {
            'L' => {
                dial = @mod(dial - rec.num, 100);
            },
            'R' => {
                dial = @mod(dial + rec.num, 100);
            },
            else => {
                std.log.err("Unknown direction: {}\n", .{ rec.direction });
                return error.InvalidInput;
            }
        }

        if (dial == 0) {
            num_zero += 1;
        }
    }

    return num_zero;
}

pub fn part2(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const records = try parseInput(allocator, input);
    defer allocator.free(records);

    var dial: i32 = 50;
    var num_zero: u32 = 0;
    for (records) |rec| {
        const num_rotations: u32 = @intCast(@divFloor(rec.num, 100));
        num_zero += num_rotations; // Always add the number of full rotations

        const remainder: i32 = @rem(rec.num, 100);
        switch (rec.direction) {
            'L' => {
                if (remainder >= dial and dial != 0) {
                    num_zero += 1; // Rotated through 0
                }
                dial = @mod(dial - remainder, 100);
            },
            'R' => {
                if (dial + remainder >= 100 and dial != 0) {
                    num_zero += 1; // Rotated through 0
                }
                dial = @mod(dial + remainder, 100);
            },
            else => {
                std.log.err("Unknown direction: {}\n", .{ rec.direction });
                return error.InvalidInput;
            }
        }
    }

    return num_zero;
}
