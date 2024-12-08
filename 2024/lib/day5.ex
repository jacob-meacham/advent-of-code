defmodule Lists do
  @moduledoc """
  Helper for list manipulation
  """

  def middle(list) when is_list(list) and length(list) > 0 do
    mid_index = div(length(list), 2)
    Enum.at(list, mid_index)
  end
end

defmodule Advent.Day5.LaunchSafetyManual do
  use Advent.Day, no: 5
  @moduledoc "Day 5"

  defp parse_input(input) do
    [rule_list, orders] = String.split(input, "\n\n", trim: true)

    rules = rule_list
      |> String.split("\n", trim: true)
      |> Enum.reduce(%{}, fn rule, acc ->
        [a, b] = rule |> String.split("|", trim: true)
        Map.update(acc, a, MapSet.new([b]), &MapSet.put(&1, b))
      end)

      orders =
        orders
      |> String.split("\n", trim: true)
      |> Enum.map(&String.split(&1, ",", trim: true))

      {rules, orders}
  end

  defp order_valid?([o], _) when length([o]) == 1, do: true
  defp order_valid?([first | rest], rules) do
    s = MapSet.new(rest)
    rule_set = rules |> Map.get(first, MapSet.new())
    diff = MapSet.difference(s, rule_set)

    case MapSet.size(diff) do
      0 -> order_valid?(rest, rules)
      _ -> false
    end
  end

  def part1(input) do
    {rules, orders} = parse_input(input)

    orders
      |> Enum.map(fn order_set ->
        case order_valid?(order_set, rules) do
          true -> Lists.middle(order_set) |> String.to_integer()
          false -> 0
        end
      end) |> Enum.sum()
  end

  defp make_valid(order, rules) do
    all_pages = MapSet.new(order)
    order
      |> Enum.map(&{MapSet.size(MapSet.difference(all_pages, Map.get(rules, &1, MapSet.new()))), &1})
      |> Enum.sort_by(fn {c1, _} -> c1 end)
      |> Enum.map(fn {_, l} -> l end)
  end

  def part2(input) do
    {rules, orders} = parse_input(input)

    orders
      |> Enum.reduce([], fn order_set, acc ->
        case order_valid?(order_set, rules) do
          true -> acc
          false -> acc ++ [order_set]
        end
      end)
      |> Enum.map(fn order ->
          make_valid(order, rules)
        end)
      |> Enum.map(fn l -> Lists.middle(l) |> String.to_integer() end)
      |> Enum.sum()
  end

  def run_part1() do input() |> part1() end
  def run_part2() do input() |> part2() end
end

# contents = Advent.Day.input(5)
# IO.puts(LaunchSafetyManual.part1(contents))
# IO.puts(LaunchSafetyManual.part2(contents))

# Benchee.run(%{
#   day5_part1: fn -> contents |> LaunchSafetyManual.part1() end,
#   day5_part2: fn -> contents |> LaunchSafetyManual.part2() end,
#   day5_total: fn ->
#     LaunchSafetyManual.part1(contents)
#     LaunchSafetyManual.part2(contents)
#   end}, warmup: 2,
# time: 3)
