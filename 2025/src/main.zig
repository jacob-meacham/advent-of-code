const std = @import("std");
const day1 = @import("day_1.zig");
const day2 = @import("day_2.zig");
const day3 = @import("day_3.zig");
const day4 = @import("day_4.zig");
const day5 = @import("day_5.zig");
const day6 = @import("day_6.zig");
const day7 = @import("day_7.zig");
const day8 = @import("day_8.zig");
const day9 = @import("day_9.zig");
const day10 = @import("day_10.zig");
const day11 = @import("day_11.zig");
const day12 = @import("day_12.zig");

const Action = enum {
    bench,
    day
};

const PartFn = *const fn(std.mem.Allocator, []const u8) anyerror!i64;

const Day = struct {
    dayNum: u8,
    part1: PartFn,
    part2: PartFn,

    pub fn inputPath(self: *const Day, allocator: std.mem.Allocator) ![]const u8 {
        return try std.fmt.allocPrint(allocator, "src/input/day{}.txt", .{ self.dayNum });
    }
};

const days: []const Day = &[_]Day {
    .{ .dayNum = 1, .part1 = day1.part1, .part2 = day1.part2 },
    .{ .dayNum = 2, .part1 = day2.part1, .part2 = day2.part2 },
    .{ .dayNum = 3, .part1 = day3.part1, .part2 = day3.part2 },
    .{ .dayNum = 4, .part1 = day4.part1, .part2 = day4.part2 },
    .{ .dayNum = 5, .part1 = day5.part1, .part2 = day5.part2 },
    .{ .dayNum = 6, .part1 = day6.part1, .part2 = day6.part2 },
    .{ .dayNum = 7, .part1 = day7.part1, .part2 = day7.part2 },
    .{ .dayNum = 8, .part1 = day8.part1, .part2 = day8.part2 },
    .{ .dayNum = 9, .part1 = day9.part1, .part2 = day9.part2 },
    .{ .dayNum = 10, .part1 = day10.part1, .part2 = day10.part2 },
    .{ .dayNum = 11, .part1 = day11.part1, .part2 = day11.part2 },
    .{ .dayNum = 12, .part1 = day12.part1, .part2 = day12.part2 }
};

fn readInput(allocator: std.mem.Allocator, path: []const u8) ![]u8 {
    return try std.fs.cwd().readFileAlloc(
        allocator,
        path,
        std.math.maxInt(usize),
    );
}

pub fn benchmarkDay(allocator: std.mem.Allocator, day: Day) ![2]f64 {
    const iterations = 10;
    const funcs: [2]PartFn = .{ day.part1, day.part2 };

    const inputPath = try day.inputPath(allocator);
    defer allocator.free(inputPath);

    const input = try readInput(allocator, inputPath);
    defer allocator.free(input);

    var partAverages: [2]f64 = .{0.0, 0.0};
    for (funcs, 0..) |func, index| {
        _ = try func(allocator, input); // simple warmup

        var timer = try std.time.Timer.start();
        for (0..iterations) |_| {
            _ = try func(allocator, input);
        }

        const elapsed = timer.read(); // ns
        const ms = @as(f64, @floatFromInt(elapsed)) / 1_000_000.0;
        partAverages[index] = ms / iterations;
    }

    return partAverages;
}

fn usage(name: []const u8) noreturn {
    std.debug.print("Usage: {s} <day|bench> <num>\n", .{ name });
    std.process.exit(1);
}

fn printBenchmarkHeader() void {
    std.debug.print(
        "| {s:<6} | {s:<11} | {s:<11} | {s:<10} | {s:<5} |\n",
        .{ "Day", "Part 1 (ms)", "Part 2 (ms)", "Total (ms)", "Good?" },
    );
    std.debug.print("|--------|-------------|-------------|------------|-------|\n", .{});
}

fn printBenchmarkRow(day_num: usize, p1: f64, p2: f64) void {
    const total = p1 + p2;
    const good = if (total < 100.0) "✅" else "❌";

    std.debug.print(
        "| Day {d:<2} | {d:>11.2} | {d:>11.2} | {d:>10.2} | {s:<6} |\n",
        .{ day_num, p1, p2, total, good },
    );
}

fn printBenchmarkFooter() void {
    std.debug.print("|--------|-------------|-------------|------------|-------|\n", .{});
}

pub fn main() !void {
    // var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    // defer {
    //     const leaked = gpa.deinit();
    //     if (leaked == .leak) {
    //         std.log.err("MEMORY LEAK DETECTED!", .{});
    //         std.process.exit(1);
    //     }
    // }
    // const allocator = gpa.allocator();

    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    defer arena.deinit(); // frees everything at once

    const allocator = arena.allocator();

    const args = try std.process.argsAlloc(allocator);
    defer std.process.argsFree(allocator, args);

    if (args.len < 2) {
        usage(args[0]);
    }

    const action = args[1];
    const action_enum = std.meta.stringToEnum(Action, action)
        orelse {
            usage(args[0]);
        };

    var dayToRun: ?u8 = null;
    if (args.len > 2) {
        dayToRun = try std.fmt.parseInt(u8, args[2], 10);
    }

    switch (action_enum) {
        .bench => {
            if (dayToRun) | dayNum | {
                const avgMs = try benchmarkDay(allocator, days[dayNum-1]);
                std.debug.print("Day {d}: Part1 - {d:.3} ms, Part 2 - {d:.3} ms\n",
                    .{ dayNum, avgMs[0], avgMs[1] });
            } else {
                printBenchmarkHeader();
                for (days) |day| {
                    const avgMs = try benchmarkDay(allocator, day);
                    printBenchmarkRow(day.dayNum, avgMs[0], avgMs[1]);
                }
                printBenchmarkFooter();
            }
        },
        .day => {
            const dayNum = dayToRun orelse usage(args[0]);

            const day = days[dayNum-1];
            const inputPath = try day.inputPath(allocator);
            defer allocator.free(inputPath);

            const input = try readInput(allocator, inputPath);
            defer allocator.free(input);

            const val1 = try day.part1(allocator, input);
            const val2 = try day.part2(allocator, input);

            std.debug.print("Part 1: {}\nPart 2: {}\n", .{ val1, val2 });
        }
    }
}
