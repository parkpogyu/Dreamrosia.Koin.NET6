#!/bin/bash
#directory=${PWD}
directory=~/MiningBot
for i in $(seq -f "%03g" 1 20)
do
	path="$directory/bots/bot_$i"
	cd $path
	echo "/usr/bin/dotnet $path/mining-bot.dll &"
	/usr/bin/dotnet "$path"/mining-bot.dll &
	#dotnet mining-bot.dll &
done

