# Module Context Guide

## Purpose

Define how module-level context is used while the source remains a flat plugin
project.

## Audience

AI agents changing a feature boundary.

## Update when

Update when a logical module gains or loses responsibilities.

## Depends on

[`AGENTS.md`](../../AGENTS.md), [`architecture/overview.md`](../../architecture/overview.md)
and the individual module pages in this directory.

## Current status

These pages are context companions, not claims that the source has already been
physically split. Source changes must still be mapped to the current files and
task classes.

## Required module questions

- What business responsibility does the module own?
- Which state does it read or mutate?
- Which DPB/API boundary does it touch?
- Which task owns its cycle?
- What fallback and postcondition exist?
- Which documents and tests must change with it?
