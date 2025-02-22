cd /c/enterbot
git checkout master
git push
git checkout gh-pages
git merge master --no-edit
#check that automatic merge worked without conflicts - continue only if it did
if [ $(git ls-files -u | wc -l) -eq 0 ]; then
    of=/c/enterbot/UI/bin/Release/net9.0/publish
    rm -rf $of
    dotnet publish --configuration Release -o $of /c/enterbot/UI/ui.csproj
    cp $of/wwwroot/* /c/enterbot/ -r
    sed -i 's/<base href="\/"\s*\/>/<base href="\/enterbot\/">/' /c/enterbot/index.html
    sed -i 's/<base href="\/"\s*\/>/<base href="\/enterbot\/">/' /c/enterbot/404.html
    git add .
    git commit -m "Merge master into gh-pages and publish"
    git push
    git checkout master
else
    echo "Merge conflicts detected. Aborting deployment."
    git merge --abort
    git checkout master
fi