const std = @import("std");

const Range = struct {
    lower: u64,
    upper: u64,

    pub fn parse(line: []const u8) !Range {
        var ranges = std.mem.splitScalar(u8, line, '-');
        const lower = try std.fmt.parseInt(u64, ranges.next() orelse return error.InvalidInput, 10);
        const upper = try std.fmt.parseInt(u64, ranges.next() orelse return error.InvalidInput, 10);

        return .{
            .lower = lower,
            .upper = upper
        };
    }

    pub fn compare(_: void, lhs: Range, rhs: Range) bool {
        return lhs.lower < rhs.lower;
    }
};

fn parseIngredientRanges(allocator: std.mem.Allocator, input: []const u8) ![] Range {
    var ingredient_ranges = try std.ArrayList(Range).initCapacity(allocator, 200);
    errdefer ingredient_ranges.deinit(allocator);

    var lines = std.mem.splitScalar(u8, input, '\n');

    while (lines.next()) |line| {
        try ingredient_ranges.append(allocator, try Range.parse(line));
    }

    return ingredient_ranges.toOwnedSlice(allocator);
}

fn parseIngredients(allocator: std.mem.Allocator, input: []const u8) ![]const u64 {
    var ingredients = try std.ArrayList(u64).initCapacity(allocator, 200);
    errdefer ingredients.deinit(allocator);

    var lines = std.mem.splitScalar(u8, input, '\n');

    while (lines.next()) |line| {
        try ingredients.append(allocator, try std.fmt.parseInt(u64, line, 10));
    }

    return ingredients.toOwnedSlice(allocator);
}

fn parseInput(allocator: std.mem.Allocator, input: []const u8) !struct { [] Range, []const u64 } {
    var sections = std.mem.tokenizeSequence(u8, input, "\n\n");
    const range_section = sections.next() orelse return error.InvalidInput;
    const ingredient_section = sections.next() orelse return error.InvalidInput;
    if (sections.next()) |_| {
        return error.InvalidInput;
    }

    const ingredient_ranges = try parseIngredientRanges(allocator, range_section);
    errdefer allocator.free(ingredient_ranges);

    const ingredients = try parseIngredients(allocator, ingredient_section);
    errdefer allocator.free(ingredients);

    return .{ ingredient_ranges, ingredients };
}

// Assumes sorted
fn mergeRanges(allocator: std.mem.Allocator, ranges: []const Range) ![]Range {
    var merged = try std.ArrayList(Range).initCapacity(allocator, ranges.len);
    errdefer merged.deinit(allocator);

    try merged.append(allocator, ranges[0]); // Start with the first
    for (ranges[1..]) |range| {
        const last = merged.items.len-1;
        if (range.lower <= merged.items[last].upper) {
            merged.items[last].upper = @max(range.upper, merged.items[last].upper);
        } else {
            try merged.append(allocator, range);
        }
    }

    return merged.toOwnedSlice(allocator);
}

const SearchContext = struct {
    ingredient: u64,

    fn compare(self: @This(), item: Range) std.math.Order {
        return std.math.order(self.ingredient, item.lower);
    }
};

pub fn part1(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const ranges, const ingredients = try parseInput(allocator, input);
    defer allocator.free(ranges);
    defer allocator.free(ingredients);

    std.mem.sort(Range, ranges, {}, Range.compare);
    const merged = try mergeRanges(allocator, ranges);
    defer allocator.free(merged);

    var num_fresh: usize = 0;
    for (ingredients) |ingredient| {
        const i = std.sort.upperBound(
            Range,
            merged,
            SearchContext{.ingredient = ingredient},
            SearchContext.compare
        );
        if (i == 0) {
            continue;
        }

        num_fresh += @intFromBool(ingredient <= merged[i-1].upper);
    }

    return @intCast(num_fresh);
}

pub fn part2(allocator: std.mem.Allocator, input: []const u8) !i64 {
    const ranges, const ingredients = try parseInput(allocator, input);
    defer allocator.free(ranges);
    defer allocator.free(ingredients);

    std.mem.sort(Range, ranges, {}, Range.compare);
    const merged = try mergeRanges(allocator, ranges);
    defer allocator.free(merged);

    var num_fresh: usize = 0;
    for (merged) |range| {
        num_fresh += range.upper-range.lower+1;
    }

    return @intCast(num_fresh);
}
