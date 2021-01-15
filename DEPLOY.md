# Deployment of new UFrameIT Releases

TODO: describe how to deploy here

## Versioning

We use semantic versioning. A version number looks like `x.y.z`, no `v` prefix.

Always release all of

- [UFrameIT/UFrameIT](https://github.com/UFrameIT/UFrameIT) (this repo)
- [FrameIT/frameworld](https://gl.mathhub.info/FrameIT/frameworld) (the MMT archive)
- [UFrameIT/archives](https://github.com/UFrameIT/archives) (Git aggregation repo of MMT archives)

at the same time with the *same* version number.

Concretely:

1. Merge [FrameIT/frameworld](https://gl.mathhub.info/FrameIT/frameworld)'s devel branch into master and git-tag with `x.y.z`.
2. Update archive submodules in [UFrameIT/archives](https://github.com/UFrameIT/archives) and git-tag with `x.y.z`.
3. Git-tag [UFrameIT/UFrameIT](https://github.com/UFrameIT/UFrameIT) (this repo) with `x.y.z.`.
4. Create a new GitHub release here: <https://github.com/UFrameIT/UFrameIT/releases>
5. Write a news post on our website: <https://github.com/UFrameIT/UFrameIT.github.io>
