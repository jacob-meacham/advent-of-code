pub fn countDigits(num: u64) usize {
    var n = num;

    var count: usize = 0;
    while (n > 0) {
        n = n / 10;
        count += 1;
    }
    return count;
}