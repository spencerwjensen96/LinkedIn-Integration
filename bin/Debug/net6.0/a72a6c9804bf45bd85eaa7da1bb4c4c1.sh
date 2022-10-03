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

ps 51725;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 51725 > /dev/null;
done;

for child in $(list_child_processes 51744);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/spencerjensen/Desktop/practice/linkedintest/bin/Debug/net6.0/a72a6c9804bf45bd85eaa7da1bb4c4c1.sh;
