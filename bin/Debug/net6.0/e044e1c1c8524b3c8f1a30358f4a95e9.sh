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

ps 44296;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 44296 > /dev/null;
done;

for child in $(list_child_processes 44310);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/spencerjensen/Desktop/practice/linkedintest/bin/Debug/net6.0/e044e1c1c8524b3c8f1a30358f4a95e9.sh;
