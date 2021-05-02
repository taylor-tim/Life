using System;
using System.Collections.Generic;

namespace GameofLife {
    class Program {
        private static HashSet<string> alive = new HashSet<string>();

        static void Main(string[] args) {
            // Read in user inputs
            List<string> userInputs = ReadInputs();

            // Parse those inputs
            ParseInputs(userInputs);

            // Run 10 iterations
            for (int i = 0; i < 10; i++) {
                RunIteration();
            }

            // Print out the result
            PrintResult();
        }

        static List<string> ReadInputs() {
            // Take in user inputs from stdin. Ends on empty line
            string inputLine;
            var userInputs = new List<string>();
            while ((inputLine = Console.ReadLine()) != "") {
                userInputs.Add(inputLine);
            }
            return userInputs;
        }

        static void ParseInputs(List<string> userInputs) {
            /* This basically just skips comment lines. It used to do more, but turns out
               user-defined classes aren't consistent in hashing, so saving as str is a thing.
               I'd normally find a way to overwrite whatever the hashing method is in C# so
               they can check membership consistently, but I'm afraid 3 hours isn't enough time for me
               to figure out what that is and test it all out. 
            */
            foreach (string inputLine in userInputs) {
                if (inputLine.StartsWith("#")) {
                    continue;
                }

                alive.Add(inputLine);
            }
        }

        private static HashSet<string> GetNeighbors(string inputCoord) {
            HashSet<string> neighbors = new HashSet<string>();

            // Not a fan of doing this repeatedly, but running short on time after hash experiment
            string[] pairs = inputCoord.Split(' ');
            long row = Int64.Parse(pairs[0]);
            long col = Int64.Parse(pairs[1]);

            List<long> validRows = new List<long>() { row };
            List<long> validCols = new List<long>() { col };

            // Don't allow wraps arounds
            if (row != Int64.MaxValue) {
                validRows.Add(row + 1);
            }
            if (row != Int64.MinValue) {
                validRows.Add(row - 1);
            }
            if (col != Int64.MaxValue) {
                validCols.Add(col + 1);
            }
            if (col != Int64.MinValue) {
                validCols.Add(col - 1);
            }

            // Not sure if there's a stdlib way to get permutations in C#, so long way.
            foreach (long x in validRows) {
                foreach (long y in validCols) {
                    string newCoord = $"{x} {y}";
                    // Let's not add back the original coord as a neighbor to itself
                    if (newCoord != inputCoord) {
                        neighbors.Add(newCoord);
                    }
                }
            }

            return neighbors;
        }

        private static void RunIteration() {
            // Get all alive cells and all their neighbors to see what happens this iteration
            HashSet<string> cellsToCheck = GetPossibleCells();

            // New hashset to store the new config in
            HashSet<string> newAlive = new HashSet<string>();

            foreach (string coord in cellsToCheck) {
                if (ShouldBeOn(coord)) {
                    newAlive.Add(coord);
                }
            }
            // Overwrite the previous config with the new one
            alive = newAlive;
        }

        private static int AliveNeighborCount(string coord) {
            // Check how many neighbors are on/alive
            HashSet<string> neighbors = GetNeighbors(coord);
            int aliveCount = 0;

            foreach (string neighbor in neighbors) {
                if (alive.Contains(neighbor)) {
                    aliveCount += 1;
                }
            }

            return aliveCount;
        }

        private static Boolean ShouldBeOn(string coord) {
            // Check a cell to see if it _should_ be on next iteration
            int aliveNeighbors = AliveNeighborCount(coord);
            Boolean isOn = alive.Contains(coord);

            switch (aliveNeighbors) {
                // Exactly 3, turns on or stays on
                case 3:
                    return true;
                // For 2, off stays off, on stays on, so keep whatever current state is
                case 2:
                    return isOn;
                // All other cases should be off
                default:
                    return false;
            }
        }

        private static HashSet<string> GetPossibleCells() {
            // Return union of alive currently and all their neighbors
            // (No non-neighbors will change, so no need to expand farther)
            HashSet<string> cellsToCheck = new HashSet<string>();

            foreach(string coord in alive) {
                string[] pairs = coord.Split(' ');
                long row = Int64.Parse(pairs[0]);
                long col = Int64.Parse(pairs[1]);

                HashSet<string> neighbors = GetNeighbors($"{row} {col}");
                cellsToCheck.UnionWith(neighbors);
            }

            return cellsToCheck;
        }
        private static void PrintResult() {
            // Print out the state of the board
            Console.WriteLine("#Life 1.06");
            foreach (string coord in alive) {
                Console.WriteLine(coord);
            }
        }
    }
}
