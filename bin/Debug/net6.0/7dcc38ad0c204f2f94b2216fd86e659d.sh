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

ps 42114;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 42114 > /dev/null;
done;

for child in $(list_child_processes 42122);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/spencerjensen/Desktop/practice/linkedintest/bin/Debug/net6.0/7dcc38ad0c204f2f94b2216fd86e659d.sh;
