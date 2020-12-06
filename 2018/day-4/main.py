from datetime import datetime
import argparse
import sys
import functools
import re
import itertools

class GuardRecord(object):
    def __init__(self, guard_id, events):
        self.id = guard_id
        self._events = events


        # Number of minutes this guard was asleep, bucketed by midnight minute
        self._sleep_minutes = self._calc_sleep_time()

    def _calc_sleep_time(self):
        sleep_minutes = [0] * 60
        def increment_slice(_from, to, arr):
            arr[_from:to] = [s + 1 for s in arr[_from:to]]

        for day_events in itertools.groupby(self._events, lambda e: e['date'].day):
            # These events are already sorted by time, as the group by doesn't change the sort order
            last_minute = 0
            is_asleep = False # An alternative to tracking asleepness would be to always append a final event at 1:00 am
            for event in day_events[1]:
                if not is_asleep and 'falls asleep' in event['data']:
                    is_asleep = True
                    last_minute = event['date'].minute
                elif is_asleep and 'wakes' in event['data']:
                    # Was sleeping from last_minute until now
                    increment_slice(last_minute, event['date'].minute, sleep_minutes)
                    last_minute = event['date'].minute
                    is_asleep = False

            if is_asleep:
                # Track the final minutes as asleep:
                increment_slice(last_minute, 60, sleep_minutes)

        return sleep_minutes



    @property
    def time_spent_sleeping(self):
        return sum(self._sleep_minutes)


    @property
    def most_sleepy_minute(self):
        # 2 pass solution, but it's only 60 elements and could be cached. Plus it feels...PYTHONIC!
        return self._sleep_minutes.index(max(self._sleep_minutes))

    @property
    def max_sleepy_minute(self):
        return max(self._sleep_minutes)

def get_events(infile):
    events = [{ 'date': datetime.strptime(d, '[%Y-%m-%d %H:%M'), 'data': e.strip() }
              for d, e in [raw_event.split('] ') for raw_event in infile.readlines()]]
    return sorted(events, key= lambda e: e['date'])

def part1(guard_records):
    # Find the sleepiest guard
    sleepiest_guard = sorted(guard_records, reverse=True, key=lambda g: g.time_spent_sleeping)[0]
    return int(sleepiest_guard.id) * sleepiest_guard.most_sleepy_minute

def part2(guard_records):
    # Find the guard who was most frequently asleep on the same minute
    sleepiest_guard = sorted(guard_records, reverse=True, key=lambda g: g.max_sleepy_minute)[0]

    return int(sleepiest_guard.id) * sleepiest_guard.most_sleepy_minute

def main(infile):
    # # Parse and sort the events
    # events = get_events(infile)
    #
    # # Bucket events by guards
    # guard_regex = re.compile('Guard #(\d+) begins shift')
    # def bucket_by_guards(acc, event):
    #     guard_event = guard_regex.match(event['data'])
    #     if guard_event:
    #         acc['cur_guard'] = guard_event.group(1)
    #
    #     acc['events'].setdefault(acc['cur_guard'], []).append(event)
    #     return acc
    #
    # events_by_guards = functools.reduce(bucket_by_guards, events, { 'cur_guard': None, 'events': { } })
    # guard_records = [GuardRecord(_id, events) for _id, events in events_by_guards['events'].items()]
    #
    # return {
    #     'part1': part1(guard_records),
    #     'part2': part2(guard_records)
    # }

    def isMatch(a, b):
        return a != b and a.lower() == b.lower()

    # TODO: instead of checking each one each time, I only have to go back one character and check it that is now a match.

    polymer = list(infile.readline())
    stack = []
    i = 0
    for c in polymer:
        if len(stack) and isMatch(c, stack[-1]):
            stack.pop()
        else:
            stack.append(c)

    #
    # while i < len(polymer):
    #
    #     if i == cur_len-1:
    #         new_data.append(cur_data[i])
    #         i += 1
    #     elif not isMatch(cur_data[i], cur_data[i+1]):
    #         new_data.append(cur_data[i])
    #         i += 1
    #     else:
    #         i += 2
    # cur_len = len(cur_data)
    # num_cycles = 0
    # while True:
    #     new_data = []
    #     i = 0
    #     while i < cur_len:
    #         if i == cur_len-1:
    #             new_data.append(cur_data[i])
    #             i += 1
    #         elif not isMatch(cur_data[i], cur_data[i+1]):
    #             new_data.append(cur_data[i])
    #             i += 1
    #         else:
    #             i += 2
    #
    #     if len(new_data) == cur_len:
    #         # No change was made, so we're done.
    #         break
    #
    #     cur_data = new_data
    #     cur_len = len(new_data)
    #     num_cycles += 1

    return len(stack)



def parse_args():
    parser = argparse.ArgumentParser()
    parser.add_argument('infile', type=argparse.FileType('r'), default=sys.stdin)

    return parser.parse_args()

if __name__ == '__main__':
    options = parse_args()

    print(main(options.infile))
