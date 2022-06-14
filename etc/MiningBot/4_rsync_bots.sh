#!/bin/bash

#echo "find ~/MiningBot/bots ! -name 'bot.guid' -type f -exec rm -f {} +"
#find ~/MiningBot/bots ! -name 'bot.guid' -type f -exec rm -f {} +

for i in $(seq -f "%03g" 1 20)
do
	echo "rsync -auh ~/MiningBot/deploy/Publsh/ ~/MiningBot/bots/bot_$i"
	rsync -auh ~/MiningBot/deploy/Publish/ ~/MiningBot/bots/bot"_$i"
done

