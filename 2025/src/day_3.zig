const std = @import("std");

fn parseInput(allocator: std.mem.Allocator, input: []const u8) ![][]const u8 {
    var banks = try std.ArrayList([]const u8).initCapacity(allocator, 100);
    errdefer banks.deinit(allocator);

    var reader: std.io.Reader = .fixed(input);
    while (true) {
        const mline = try reader.takeDelimiter('\n');
        if (mline) |line| {
            var batteries = try std.ArrayList(u8).initCapacity(allocator, 100);
            errdefer batteries.deinit(allocator);

            for (line) |b| {
                const v: u8 = b - '0';
                try batteries.append(allocator, v);
            }

            try banks.append(allocator, try batteries.toOwnedSlice(allocator));
        } else break;
    }

    return banks.toOwnedSlice(allocator);
}

fn getMax(bank: []const u8) u8 {
    const head = bank[0];
    const tail = bank[1..];
    if (tail.len == 0) {
        return head;
    }

    const candidate = getMax(tail);
    if (candidate > head) return candidate else return head;
}

fn getMaxJoltage(bank: []const u8) u64 {
    var max_so_far: u64 = 0;
    for(0..bank.len-1) |index| {
        const candidate = bank[index] * 10 + getMax(bank[index+1..]);
        max_so_far = @max(candidate, max_so_far);
    }
    return max_so_far;
}

fn arrToInt(arr: []const u8) u64 {
    var res: u64 = 0;
    for (arr) |n| {
        res = res * 10 + n;
    }

    return res;
}

fn getMaxJoltageInWindow(allocator: std.mem.Allocator, bank: []const u8, batteriesNeeded: u8) !u64 {
    var result = try std.ArrayList(u8).initCapacity(allocator, batteriesNeeded);
    defer result.deinit(allocator);

    var current_index: usize = 0;
    while (result.items.len < batteriesNeeded) {
        const end = bank.len - (batteriesNeeded - result.items.len);

        var max_digit: u8 = 0;
        var best_pos: usize = 0;

        for (current_index..end+1) |i| {
            const candidate = bank[i];
            if (candidate > max_digit) {
                max_digit = candidate;
                best_pos = i;

                if (max_digit == 9) {
                    break; // Can't do any better
                }
            }
        }

        try result.append(allocator, max_digit);
        current_index = best_pos + 1;
    }

    return arrToInt(result.items);
}

fn bankDeinit(allocator: std.mem.Allocator, banks: [][]const u8) void {
    for (banks) |bank| allocator.free(bank);
    allocator.free(banks);
}

pub fn part1(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const banks = try parseInput(allocator, input);
    defer bankDeinit(allocator, banks);

    var joltage_sum: u64 = 0;
    for (banks) |bank| {
        joltage_sum += getMaxJoltage(bank);
    }
    return @intCast(joltage_sum);
}

pub fn part2(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const banks = try parseInput(allocator, input);
    defer bankDeinit(allocator, banks);

    var joltage_sum: u64 = 0;
    for (banks) |bank| {
        joltage_sum += try getMaxJoltageInWindow(allocator, bank, 12);
    }
    return @intCast(joltage_sum);
}
