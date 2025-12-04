const std = @import("std");

const IdNumber = struct {
    low: u64,
    high: u64,
    num: u64,
    count: usize,

    pub fn parse(line: []const u8) !IdNumber {
        const count = line.len;
        if (count == 1) {
            // edge case for single digit
            const num = try std.fmt.parseInt(u64, line, 10);
            return .{
                .low = num,
                .high = 0,
                .num = num,
                .count = count
            };
        }

        const high = try std.fmt.parseInt(u64, line[0..count/2], 10);
        const low = try std.fmt.parseInt(u64, line[count/2..], 10);
        const num = try std.fmt.parseInt(u64, line, 10);

        return .{
            .low = low,
            .high = high,
            .num = num,
            .count = count
        };
    }
};

fn parseInput(allocator: std.mem.Allocator, input: []const u8) ![]const struct { IdNumber, IdNumber} {
    var idRanges = try std.ArrayList(struct { IdNumber, IdNumber }).initCapacity(allocator, 50);
    errdefer idRanges.deinit(allocator);

    var reader: std.io.Reader = .fixed(input);
    while (true) {
        const mline = try reader.takeDelimiter(',');
        if (mline) |line| {
            var it = std.mem.splitScalar(u8, line, '-');

            const first_str = it.next() orelse return error.InvalidFormat;
            const second_str = it.next() orelse return error.InvalidFormat;

            const lower = try IdNumber.parse(first_str);
            const upper = try IdNumber.parse(second_str);

            try idRanges.append(allocator, .{ lower, upper} );
        } else break;
    }

    return idRanges.toOwnedSlice(allocator);
}

fn countDigits(num: u64) usize {
    var n = num;

    var count: usize = 0;
    while (n > 0) {
        n = n / 10;  // or just n / 10 for positive numbers
        count += 1;
    }
    return count;
}

fn repeatNumber(n: u64, times: usize) u64 {
    const digits = countDigits(n);
    const shift = std.math.pow(u64, 10, digits);

    var res: u64 = 0;
    for (0..times) |_| {
        res = res * shift + n;
    }
    return res;
}

fn exactInvalidInRange(lower: IdNumber, upper: IdNumber) u64 {
    var invalidSum: u64 = 0;

    // If the lower number is odd, then move to the next power of 10
    var startingNumber =  if (lower.count % 2 != 0) std.math.pow(u64, 10, lower.count / 2) else lower.high;
    // If the higher number is odd, then only count to the previous power of 10
    const endingNumber = if (upper.count % 2 != 0) std.math.pow(u64, 10, upper.count / 2) else upper.high;

    if (lower.count % 2 == 0 and startingNumber < endingNumber) {
        // We have a partial starting range that we need to check - eg 7772-100000 will allow for 7777 but 7778-100000 will not
        if (lower.high >= lower.low) {
            invalidSum += repeatNumber(lower.high, 2);
        }
        startingNumber += 1;
    }

    // Count all between start end end
    while (startingNumber < endingNumber) {
        invalidSum += repeatNumber(startingNumber, 2);
        startingNumber += 1;
    }

    // Finally, need to see if we have a partial ending range. Edge case is if the numbers are very close together (eg 12-15)
    // We need to check that the invalid is actually in the range
    const upperCandidate = repeatNumber(endingNumber, 2);
    if (lower.num <= upperCandidate and upperCandidate <= upper.num) {
        invalidSum += upperCandidate;
    }

    return invalidSum;
}

fn allInvalidInRange(allocator: std.mem.Allocator, lower: IdNumber, upper: IdNumber) !u64 {
    var invalidIds = std.AutoHashMap(u64, void).init(allocator);
    defer invalidIds.deinit();

    var candidateNum: u64 = 1;
    while (true) {
        const candidateDigits = countDigits(candidateNum);
        if (candidateDigits > upper.count/2) {
            break;
        }

        // Always start from at least the length of the lower
        var totalTimes: usize = @max(lower.count / candidateDigits, 2);
        if (totalTimes * candidateDigits < lower.count) {
            totalTimes += 1;
        }

        while (true) {
            const invalidIdCandidate = repeatNumber(candidateNum, totalTimes);

            if (lower.num <= invalidIdCandidate and invalidIdCandidate <= upper.num) {
                try invalidIds.put(invalidIdCandidate, {});
            }

            totalTimes += 1;
            if (invalidIdCandidate > upper.num) {
                break;
            }
        }
        candidateNum += 1;
    }

    var invalidSum: u64 = 0;
    var it = invalidIds.keyIterator();
    while (it.next()) |key| {
        invalidSum += key.*;
    }

    return invalidSum;
}

pub fn part1(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const idRanges = try parseInput(allocator, input);
    var totalInvalid: u64 = 0;
    for (idRanges) | range | {
        const lower, const upper = range;
        const invalid = exactInvalidInRange(lower, upper);
        totalInvalid += invalid;
    }

    return @intCast(totalInvalid);
}

pub fn part2(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const idRanges = try parseInput(allocator, input);
    var totalInvalid: u64 = 0;
    for (idRanges) | range | {
        const lower, const upper = range;
        const invalid = try allInvalidInRange(allocator, lower, upper);
        totalInvalid += invalid;
    }

    return @intCast(totalInvalid);
}
