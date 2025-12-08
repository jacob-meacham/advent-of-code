const std = @import("std");

pub const DEFAULT_BESTS_FILE: []const u8 = ".bests";
pub const Bests = struct {
    data: std.AutoHashMap(u8, [2]f64),

    pub fn deinit(self: *@This(), allocator: std.mem.Allocator) void {
        self.data.deinit(allocator);
    }

    pub fn default(allocator: std.mem.Allocator) !Bests {
        var best = std.AutoHashMap(u8, [2]f64).init(allocator);
        errdefer best.deinit();

        return .{ .data = best };
    }

    fn parseBestLine(line: []const u8) !struct { day: u8, data: [2]f64 } {
        var parts = std.mem.splitScalar(u8, line, ',');

        const day_str = parts.next() orelse return error.InvalidFormat;
        const f1_str = parts.next() orelse return error.InvalidFormat;
        const f2_str = parts.next() orelse return error.InvalidFormat;

        const day = try std.fmt.parseInt(u8, day_str, 10);
        const f1 = try std.fmt.parseFloat(f64, f1_str);
        const f2 = try std.fmt.parseFloat(f64, f2_str);

        return .{ .day = day, .data = .{ f1, f2 } };
    }

    pub fn update(self: *@This(), day: u8, data: [2]f64) [2]f64 {
        var new_data: [2]f64 = .{ data[0], data[1] };
        if (self.data.get(day)) | old_data | {
            new_data[0] = @min(old_data[0], data[0]);
            new_data[1] = @min(old_data[1], data[1]);
        }

        self.data.put(day, new_data) catch {
            return new_data;
        };

        return new_data;
    }

    pub fn fromFile(allocator: std.mem.Allocator, path: []const u8) !Bests {
        var best = try default(allocator);

        const input = std.fs.cwd().readFileAlloc(
            allocator,
            path,
            std.math.maxInt(usize),
        ) catch |err| switch (err) {
            error.FileNotFound => return best,
            else => {
                std.log.err("Error reading bests: {s}, {}. Returning default", .{ path, err });
                return best;
            }
        };
        defer allocator.free(input);

        var lines = std.mem.splitScalar(u8, input, '\n');
        while (lines.next()) |line| {
            if (line.len == 0) {
                continue;
            }
            const entry = parseBestLine(line) catch {
                std.log.err("Couldn't parse bests line: {s}. Skipping", .{ line });
                continue;
            };

            best.data.put(entry.day, .{ entry.data[0], entry.data[1] }) catch {
                std.log.err("Trouble inserting into bests. Not good, returning what we have", .{ });
                return best;
            };
        }

        return best;
    }

    pub fn writeToFile(self: *@This(), allocator: std.mem.Allocator, path: []const u8) !void {
        var buffer = std.ArrayList(u8).initCapacity(allocator, self.data.count() * 16) catch {
            std.log.err("Error writing bests data - couldn't allocate the buffer", .{});
            return;
        };
        defer buffer.deinit(allocator);

        var keys = try std.ArrayList(u8).initCapacity(allocator, self.data.count());
        defer keys.deinit(allocator);

        var iter = self.data.keyIterator();
        while (iter.next()) |key| {
            try keys.append(allocator, key.*);
        }

        std.mem.sort(u8, keys.items, {}, std.sort.asc(u8));

        for (keys.items) |key| {
            if(self.data.getEntry(key)) |entry| {
                std.fmt.format(buffer.writer(allocator), "{},{d},{d}\n", .{
                    entry.key_ptr.*,
                    entry.value_ptr.*[0],
                    entry.value_ptr.*[1],
                }) catch {
                    std.log.err("Error writing bests data for key {}", .{ entry.key_ptr.* });
                    continue;
                };
            }
        }

        std.fs.cwd().writeFile(.{
            .sub_path = path,
            .data = buffer.items
        }) catch {
            std.log.err("Error writing bests file to {s}", .{ path });
        };
    }
};