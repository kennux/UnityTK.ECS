# Helper script that can be used to update UnityTK when using it as git submodule
# The unity tk code (excluding examples) will be copied to Assets/UnityTK after pulling newest updates from git
# The git submodule should be stored in UnityTK.

rm -Rf Assets/UnityTK.ECS
cd UnityTK.ECS
git pull origin master
cd ..
cp -Rf UnityTK.ECS/Assets/UnityTK.ECS Assets/UnityTK.ECS