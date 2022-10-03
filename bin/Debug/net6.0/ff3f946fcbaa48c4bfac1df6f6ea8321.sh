function list_child_processes(){
    local ppid=$1;
    local current_children=$(pgrep -P $ppid);
    local local_child;
    if [ $? -eq 0 ];
    then
        for current_child in $current_children
        do
          local_child=$current_child;
          list_child_processes $local_child;
          echo $local_child;
        done;
    else
      return 0;
    fi;
}

ps 43146;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 43146 > /dev/null;
done;

for child in $(list_child_processes 43151);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/spencerjensen/Desktop/practice/linkedintest/bin/Debug/net6.0/ff3f946fcbaa48c4bfac1df6f6ea8321.sh;
