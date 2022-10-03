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

ps 43533;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 43533 > /dev/null;
done;

for child in $(list_child_processes 43537);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/spencerjensen/Desktop/practice/linkedintest/bin/Debug/net6.0/b1782bea4b5b4fd49e14b7bd19672869.sh;
