const std = @import("std");

pub fn stringSort() fn (void, []const u8, []const u8) bool {
    return struct {
        fn cmp(_: void, lhs: []const u8, rhs: []const u8) bool {
            std.mem.lessThan(u8, lhs, rhs);
        }
    }.cmp;
}