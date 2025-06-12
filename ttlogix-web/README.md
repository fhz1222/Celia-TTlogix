# Project workflow

## Development

Working on a new **Feature** or **Bug fix**:

1. Create new **feature branch** with the name same as Work Item ID.
1. Implement the changes in newly created branch.
1. Update application version (see section below).
1. Merge `master` to the feature branch.
1. Create new **pull request**.

Pull request policy for `master`:

* feature branch can be merged only using ''rebase and fast-forward'' (i.e. merges with conflicts to resolve will fail)
* pull requests have to be approved by a member of `TTESA Reviewers`
* automatic pre-merge build must succeed in order to complete pull request

### Updating application version

Application version consists of 4 numbers:
```
[major].[minor].[build].[revision]
```
`major` - updated when adding or significantly changing bigger parts of the application  
`minor` - updated when a new feature is ready to be released for testing  
`build` - updated when a self-contained part of a feature is done or when introducing smaller features   
`revision` - updated when introducing minor changes and bug fixes

## CI

Each push to branch `master` triggers automatic build.

## CD

In order to release a new version of the application go to appropriate release pipeline and create new release.

## Node version - 16.4.0