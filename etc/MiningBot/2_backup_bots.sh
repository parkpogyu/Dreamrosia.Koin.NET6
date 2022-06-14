#!/bin/bash
echo "rm -rf ~/MiningBot/backup/bots"
rm -rf ~/MiningBot/backup/bots

echo "rsync -auh ~/MiningBot/bots ~/MiningBot/backup/"
rsync -auh ~/MiningBot/bots ~/MiningBot/backup/
