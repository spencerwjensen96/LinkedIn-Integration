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

ps 34542;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 34542 > /dev/null;
done;

for child in $(list_child_processes 34545);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/spencerjensen/Desktop/practice/linkedintest/bin/Debug/net6.0/cef6bdc5abc549d6a64b5297937cb8d0.sh;
