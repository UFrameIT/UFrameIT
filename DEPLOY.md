# Deployment of new UFrameIT Releases

We use semantic versioning. A version number looks like `x.y.z`, no `v` prefix.

Always release all of

- [UFrameIT/UFrameIT](https://github.com/UFrameIT/UFrameIT) (this repo)
- [FrameIT/frameworld](https://gl.mathhub.info/FrameIT/frameworld) (the MMT archive)
- [UFrameIT/archives](https://github.com/UFrameIT/archives) (Git aggregation repo of MMT archives)

at the same time with the *same* version number.

**How to make a new release**

1. Deploy prereleases on the [UFrameIT/UFrameIT GitHub repo](https://github.com/UFrameIT/UFrameIT/releases) for all OS. See below how to do that.
2. Let people test.
3. Merge [FrameIT/frameworld](https://gl.mathhub.info/FrameIT/frameworld)'s devel branch into master and git-tag with `x.y.z`.
4. Update archive submodules in [UFrameIT/archives](https://github.com/UFrameIT/archives) and git-tag with `x.y.z`.
5. Git-tag [UFrameIT/UFrameIT](https://github.com/UFrameIT/UFrameIT) (this repo) with `x.y.z.`.
6. Publish prerelease from step 1.
7. Write a news post on our website: <https://github.com/UFrameIT/UFrameIT.github.io>


**Deploying binaries for UFrameIT**

- Archive type

    - For Linux and macOS, deploy `.tar.gz` files (-> smallest file size as compared to `.zip` and `.tar`).
    - For Windows, deploy `.zip` files (-> guaranteed compatibility among end users since Windows Explorer can open them)
