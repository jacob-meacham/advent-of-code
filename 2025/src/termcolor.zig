const std = @import("std");

pub fn white(allocator: std.mem.Allocator, text: []const u8) ![]const u8 {
    return try std.fmt.allocPrint(allocator, "\x1b[37m{s}\x1b[0m", .{text});
}

pub fn brightGreen(allocator: std.mem.Allocator, value: anytype) ![]const u8 {
    return try std.fmt.allocPrint(allocator, "\x1b[92m{}\x1b[0m", .{value});
}

