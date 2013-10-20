@echo off
git push gh
git checkout no-tests
git merge master
git push aht no-tests:master
:: git push ah no-tests:master
git checkout master