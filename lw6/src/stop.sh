#! /bin/bash

cat "pid" | while read line
do
	kill $line
done
