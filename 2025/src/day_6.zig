const std = @import("std");
const math = @import("math.zig");

const Operation = enum {
    mul,
    add,
};

const Equation = struct {
  numbers: [][]const u8,
  width: usize,
  operator: Operation,
  allocator: std.mem.Allocator,

    pub fn init(allocator: std.mem.Allocator, operator: Operation, size: usize, width: usize) !Equation {
        const numbers = try allocator.alloc([]const u8, size);
        errdefer allocator.free(numbers);

        return .{
            .numbers = numbers,
            .width = width,
            .allocator = allocator,
            .operator = operator,
        };
    }

    pub fn deinit(self: @This()) void {
        self.allocator.free(self.numbers);
    }

    pub fn addNumber(self: *@This(), num: []const u8, at: usize) void {
        self.numbers[at] = num;
    }
};

fn countEquations(input: []const u8) usize {
    var lines = std.mem.splitScalar(u8, input, '\n');

    const line = lines.next() orelse return 0;
    var items = std.mem.tokenizeScalar(u8, line, ' ');

    var num_equations: usize = 0;
    while (items.next()) |_| {
        num_equations += 1;
    }

    return num_equations;
}

fn parseEquations(allocator: std.mem.Allocator, input: []const u8) ![]Equation {
    const num_equations: usize = countEquations(input);

    const equations = try allocator.alloc(Equation, num_equations);
    errdefer allocator.free(equations);

    // Count our rows and also parse the operators and column widths
    var num_rows: usize = 0;
    var line_counter = std.mem.splitScalar(u8, input, '\n');
    while (line_counter.peek()) |line| {
        if (line[0] == '*' or line[0] == '+') break;
        num_rows += 1;

        _ = line_counter.next();
    }

    const operator_line = line_counter.next() orelse return error.InvalidInput;
    const operators = try parseOperatorLine(allocator, num_equations, operator_line);
    defer allocator.free(operators);

    for (equations, operators) |*equation, operator| {
        equation.* = try Equation.init(allocator, operator.operation, num_rows, operator.width);
    }
    errdefer {
        for (equations) |eq| {
            eq.deinit();
        }
    }

    return equations;
}

fn parseNumberLine(allocator: std.mem.Allocator, widths: [] usize, line: []const u8) ![][] const u8 {
    var row = try std.ArrayList([] const u8).initCapacity(allocator, widths.len);
    errdefer row.deinit(allocator);

    var i: usize = 0;
    for (widths) |width| {
        const end = @min(i + width, line.len);
        const chunk = line[i..end];

        try row.append(allocator, chunk);
        i = end + 1;
    }

    return row.toOwnedSlice(allocator);
}

const OperationParseResult = struct {
    operation: Operation,
    width: usize,
};

fn parseOperatorLine(allocator: std.mem.Allocator, num_equations: usize, line: []const u8) ![]const OperationParseResult {
    var result = try std.ArrayList(OperationParseResult).initCapacity(allocator, num_equations);
    errdefer result.deinit(allocator);

    var i: usize = 0;
    var cur_width: usize = 0;
    var cur_operator: ?Operation = null;
    while (i < line.len) : (i += 1) {
        cur_width += 1;
        if (line[i] == ' ') {
            // Just counting
            continue;
        }

        const new_operator = switch (line[i]) {
            '*' => Operation.mul,
            '+' => Operation.add,
            else => return error.InvalidInput
        };

        if (cur_operator) |op| {
            // Remove 1 for the spaces in between
            try result.append(allocator, .{ .width = cur_width-1, .operation = op});
        }

        cur_width = 0;
        cur_operator = new_operator;
    }

    // Add the final operator and size
    if (cur_operator) |op| {
        // Add one for the operator itself
        try result.append(allocator, .{ .width = cur_width + 1, .operation = op});
    }

    return result.toOwnedSlice(allocator);
}

fn parseInput(allocator: std.mem.Allocator, input: []const u8) ![]const Equation {
    const equations = try parseEquations(allocator, input);

    var widths = try allocator.alloc(usize, equations.len);
    defer allocator.free(widths);

    for (equations, 0..) |*eq, i| {
        widths[i] = eq.width;
    }

    var lines = std.mem.splitScalar(u8, input, '\n');
    var row_index: usize = 0;
    while (lines.next()) |line| {
        if(line[0] == '*' or line[0] == '+') {
            // This is the operator line
            break;
        }

        const numbers = try parseNumberLine(allocator, widths, line);
        defer allocator.free(numbers);

        for (numbers, 0..) |num, equation_index| {
            equations[equation_index].addNumber(num, row_index);
        }

        row_index += 1;
    }

    return equations;
}

pub fn part1(allocator: std.mem.Allocator, input: []const u8) !i64 {
    var total: u64 = 0;
    const equations = try parseInput(allocator, input);
    defer {
        for (equations) |eq| {
            eq.deinit();
        }
        allocator.free(equations);
    }

    for (equations) |*equation| {
        var result: u64 = switch(equation.operator) {
            Operation.add => 0,
            Operation.mul => 1
        };

        for (equation.numbers) |num| {
            const trimmed = std.mem.trim(u8, num, " ");
            const unum = try std.fmt.parseInt(u32, trimmed, 10);
            switch(equation.operator) {
                Operation.add => result += unum,
                Operation.mul => result *= unum
            }
        }

        total += result;
    }

    return @intCast(total);
}

pub fn part2(allocator: std.mem.Allocator, input: []const u8) !i64 {
    var total: u64 = 0;

    const equations = try parseInput(allocator, input);
    defer {
        for (equations) |eq| {
            eq.deinit();
        }
        allocator.free(equations);
    }

    // Assumes all are the same length to reuse the same memory
    var new_numbers = try allocator.alloc(u64, equations[0].numbers.len);
    defer allocator.free(new_numbers);

    var num_buf = try allocator.alloc(u8, equations[0].numbers.len);
    defer allocator.free(num_buf);

    for (equations) |*equation| {
        // Read down the column
        var num_index: usize = 0;

        var w: i64 = @intCast(equation.width - 1);
        while (w >= 0) : (w -= 1) {
            var n: usize = 0;

            for (equation.numbers) |snum| {
                num_buf[n] = snum[@intCast(w)];
                n += 1;
            }

            const trimmed = std.mem.trim(u8, num_buf, " ");
            new_numbers[num_index] = try std.fmt.parseInt(u64, trimmed, 10);
            num_index += 1;
        }

        var result: u64 = switch(equation.operator) {
            Operation.add => 0,
            Operation.mul => 1
        };

        for (0..equation.width) |i| {
            switch(equation.operator) {
                Operation.add => result += new_numbers[i],
                Operation.mul => result *= new_numbers[i]
            }
        }

        total += result;
    }

    return @intCast(total);
}
