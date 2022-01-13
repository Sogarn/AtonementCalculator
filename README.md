# AtonementCalculator
A World of Warcraft 9.0 Venthyr Discipline Priest simulator to calculate atonements and stat weights for Spirit Shell build
This readme assumes knowledge of the game and class.

This simulation was designed to provide a resource for myself to improve my healing gameplay as a Discipline Priest in a raid environment in World of Warcraft.
At the time, the spirit shell build was most popular, and after trying it for a while I realized that it was mostly deterministic.
The math, however, was too complicated to solve by hand. That is where this simulator comes in.

This simulator takes in the stats of your character in the main program file and outputs a .csv file that looks like:
```
Output,Int,Stam,Crit,Haste,Mastery,Vers
-250,-41658,-75002,-11642,-18445,-10209,-15172
-200,-31535,-55547,-9717,-11856,-8418,-11904
-150,-22308,-37196,-6848,-11856,-6748,-8945
-100,-14765,-20949,-4683,-9351,-4358,-6372
-50,-7545,-9268,-2389,0,-2072,-2870
0,0,0,0,0,0,0
50,6124,5789,3061,12977,2200,2925
100,11318,11075,4971,12977,4068,5087
150,16576,12702,6987,12977,5394,7517
200,20479,11049,9033,28290,7163,9718
250,23149,10847,10486,28479,8771,11660

Atonements,Int,Stam,Crit,Haste,Mastery,Vers
-250,18,19,18,18,18,18
-200,18,19,18,18,18,18
-150,18,19,18,18,18,18
-100,18,19,18,18,18,18
-50,18,19,19,19,19,19
0,19,19,19,19,19,19
50,19,18,19,19,19,19
100,19,18,19,19,19,19
150,19,18,19,19,19,19
200,19,18,19,20,19,19
250,19,17,19,20,19,19

Weights,129.9,168.1,45.8,95.9,39,54.3,
Normalized,1,1.29,0.35,0.74,0.3,0.42,

```
The top sections is how your healing output changes as your stats change
The middle section is the optimal number of atonements to apply as stats change (in this case, the baseline is 19 targets)
The last section is stat weights, which might look familiar to anyone who has used Simulationcraft
This formatting and pre-calculation allows for easy importing to Excel.

Additional functionality includes an inidividual sim run, gear comparisons, and an atonement target count analyzer

An indiivdual sim run, which shows spell casts and a simplified current timer, might look like:
```
0s: PTW
1.3s: Bubble
2.6s: Bubble
2.6s: PTW Tick
3.9s: Bubble
3.9s: PTW Tick
5.2s: Bubble
5.2s: PTW Tick
6.4s: Bubble
7.7s: Bubble
7.7s: PTW Tick
9s: Bubble
9s: PTW Tick
10.3s: Bubble
10.3s: PTW Tick
11.6s: Bubble
12.9s: Radiance
12.9s: PTW Tick
14.6s: Radiance
14.6s: PTW Tick
16.3s: OneButtonMacro
16.3s: PTW Tick
Total healing from cast: 3789.392, per atonement 199
17.6s: Schism
17.6s: PTW Tick
17.6s: MindbenderAuto
Total healing from cast: 14396.24, per atonement 758
Total healing from cast: 3954.148, per atonement 208
Total healing from cast: 45839.44, per atonement 2413
18.9s: MindGames
18.9s: PTW Tick
18.9s: MindbenderAuto
Total healing from cast: 16359.37, per atonement 861
Total healing from cast: 5560.523, per atonement 293
Total healing from cast: 129546.2, per atonement 6818
20.2s: MindBlast
20.2s: MindbenderAuto
Total healing from cast: 18976.86, per atonement 999
Total healing from cast: 35778.68, per atonement 1988
21.5s: Penance
21.5s: MindbenderAuto
Total healing from cast: 15050.61, per atonement 836
Total healing from cast: 12622.46, per atonement 742
Total healing from cast: 13951.13, per atonement 821
22.6s: MindbenderAuto
Total healing from cast: 13741.87, per atonement 808
Total healing from cast: 13286.79, per atonement 830
23.2s: PTW
Total healing from cast: 7407.386, per atonement 494
24.5s: Smite
24.5s: MindbenderAuto
Total healing from cast: 12433.12, per atonement 829
Total healing from cast: 16392.58, per atonement 1171
25.8s: Smite
25.8s: PTW Tick
25.8s: MindbenderAuto
Total healing from cast: 9815.617, per atonement 701
Total healing from cast: 3295.125, per atonement 235
Total healing from cast: 8669.954, per atonement 1084
27.1s: Smite
27.1s: PTW Tick
27.1s: MindbenderAuto
Total healing from cast: 7928.731, per atonement 991
Total healing from cast: 2079.447, per atonement 260
Total healing from cast: 1333.839, per atonement 667
1 - Healing Recieved: 21360 Percent of Cap: 100%
2 - Healing Recieved: 21360 Percent of Cap: 100%
3 - Healing Recieved: 21360 Percent of Cap: 100%
4 - Healing Recieved: 21360 Percent of Cap: 100%
5 - Healing Recieved: 21360 Percent of Cap: 100%
6 - Healing Recieved: 21360 Percent of Cap: 100%
7 - Healing Recieved: 21360 Percent of Cap: 100%
8 - Healing Recieved: 21360 Percent of Cap: 100%
9 - Healing Recieved: 21360 Percent of Cap: 100%
10 - Healing Recieved: 21360 Percent of Cap: 100%
11 - Healing Recieved: 21360 Percent of Cap: 100%
12 - Healing Recieved: 21272.5 Percent of Cap: 99.6%
13 - Healing Recieved: 21186.8 Percent of Cap: 99.2%
14 - Healing Recieved: 21174.2 Percent of Cap: 99.1%
15 - Healing Recieved: 21163.9 Percent of Cap: 99.1%
16 - Healing Recieved: 15386.1 Percent of Cap: 72%
17 - Healing Recieved: 15211.4 Percent of Cap: 71.2%
18 - Healing Recieved: 13747.8 Percent of Cap: 64.4%
19 - Healing Recieved: 10128.5 Percent of Cap: 47.4%

Atonement Count: 19
Total Healing: 374231.2
Average Healing: 19696.4 (92.2 %)
```
The results are shown at the bottom: How much healing each target recieved, how much it was relative to the cap, and final statistics
