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

ps 59794;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 59794 > /dev/null;
done;

for child in $(list_child_processes 59804);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/spencerjensen/Desktop/code/practice/distributed-calculator-worker/distributed-calculator/bin/Debug/net6.0/0d902b7bc1cc431d8039a4169c8c7bf6.sh;
