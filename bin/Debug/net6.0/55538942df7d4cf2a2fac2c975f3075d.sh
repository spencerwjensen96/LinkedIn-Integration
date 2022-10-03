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

ps 36870;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 36870 > /dev/null;
done;

for child in $(list_child_processes 36875);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/spencerjensen/Desktop/practice/linkedintest/bin/Debug/net6.0/55538942df7d4cf2a2fac2c975f3075d.sh;
